using DnsClient;
using Microsoft.Extensions.Logging;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class VerificationService : IVerificationService
{
    private const string RecordPrefix = "_securoapi";
    private const string ValuePrefix = "securoapi-verify=";
    private static readonly TimeSpan AttemptCooldown = TimeSpan.FromSeconds(30);

    private readonly IAPIRegistryRepository _repository;
    private readonly IUserService _userService;
    private readonly ILookupClient _dns;
    private readonly ILogger<VerificationService> _logger;

    public VerificationService(IAPIRegistryRepository repository, IUserService userService, ILookupClient dns,
        ILogger<VerificationService> logger)
    {
        _repository = repository;
        _userService = userService;
        _dns = dns;
        _logger = logger;
    }


    public async Task<Result<VerificationStatusDTO>> VerifyAsync(Guid apiId, CancellationToken ct = default)
    {
        var registry = await _repository.GetByIdAsync(apiId);

        if (registry is null || registry.UserID != _userService.UserId)
            return Result<VerificationStatusDTO>.Failure(
                new Error(ErrorCodes.NotFound, $"API with the Id '{apiId}' was not found"));

        var host = new Uri(registry.TargetURL).Host;
        var recordName = $"{RecordPrefix}.{host}";
        var expected = ValuePrefix + registry.VerificationToken;

        if (registry.VerifiedAt is not null)
            return Ok(registry, recordName, expected, "Already verified.");

        if (registry.LastVerificationAttemptAt is { } last &&
            DateTime.UtcNow - last < AttemptCooldown)
            return Result<VerificationStatusDTO>.Failure(
                new Error(ErrorCodes.BadRequest, "Please wait a moment before retrying."));

        registry.LastVerificationAttemptAt = DateTime.UtcNow;
        await _repository.UpdateAsync(registry);

        IDnsQueryResponse response;
        try
        {
            response = await _dns.QueryAsync(recordName, QueryType.TXT, cancellationToken: ct);
        }
        catch (DnsResponseException ex)
        {
            _logger.LogWarning(ex, "DNS lookup failed for {RecordName} (API {APIID})", recordName, apiId);
            return Fail(registry, recordName, expected, "DNS lookup failed. Try again shortly.");
        }

        var found = response.Answers
            .TxtRecords()
            .SelectMany(r => r.Text)
            .Any(t => t.Trim() == expected);

        if (!found)
        {
            _logger.LogInformation("Verification miss for {RecordName} (API {APIID})", recordName, apiId);
            return Fail(registry, recordName, expected,
                "Record not found or value did not match. DNS changes can take a few minutes to propagate.");
        }

        registry.VerifiedAt = DateTime.UtcNow;
        registry.Status = APIStatus.Pending;
        registry.LastModifiedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(registry);

        _logger.LogInformation("Target {APIID} ({Host}) verified via DNS for user {UserId}",
            apiId, host, registry.UserID);

        return Ok(registry, recordName, expected, "Verified. Awaiting administrator approval.");
    }
    

    private static Result<VerificationStatusDTO> Ok(APIRegistry r, string name, string expected, string msg)
        => Result<VerificationStatusDTO>.Success(new VerificationStatusDTO
        {
            APIID = r.APIID, Verified = true, VerifiedAt = r.VerifiedAt,
            RecordName = name, ExpectedValue = expected, Message = msg
        });

    private static Result<VerificationStatusDTO> Fail(APIRegistry r, string name, string expected, string msg)
        => Result<VerificationStatusDTO>.Success(new VerificationStatusDTO
        {
            APIID = r.APIID, Verified = false, VerifiedAt = null,
            RecordName = name, ExpectedValue = expected, Message = msg
        });
}