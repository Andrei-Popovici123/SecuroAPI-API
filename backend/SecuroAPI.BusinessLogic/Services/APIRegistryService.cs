using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.Common.UrlNormalizer;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class APIRegistryService : IAPIRegistryService
{
    private readonly IAPIRegistryRepository _repository;
    private readonly IUserService _userService;

    public APIRegistryService(IAPIRegistryRepository repository,
        IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<Result<IEnumerable<APIRegistryDTO>>> GetAllAPIRegistriesAsync()
    {
        var registries = await _repository.GetAllAsync();
        var mappedRegistries = registries.Select(r => new APIRegistryDTO()
        {
            APIID = r.APIID,
            UserID = r.UserID,
            TargetURL = r.TargetURL,
            AuthType = r.AuthType,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt,
        });

        return Result<IEnumerable<APIRegistryDTO>>.Success(mappedRegistries);
    }

    public async Task<Result<APIRegistryDTO>> GetAPIRegistryByIdAsync(Guid id)
    {
        var registry = await _repository.GetByIdAsync(id);

        if (registry == null)
            return Result<APIRegistryDTO>
                .Failure(new Error(ErrorCodes.NotFound, $"API with the Id' {id} ' was not found"));

        return Result<APIRegistryDTO>.Success(new APIRegistryDTO
        {
            APIID = registry.APIID,
            UserID = registry.UserID,
            TargetURL = registry.TargetURL,
            AuthType = registry.AuthType,
            Status = registry.Status.ToString(),
            CreatedAt = registry.CreatedAt,
            LastModifiedAt = registry.LastModifiedAt,
        });
    }

    public async Task<Result<IEnumerable<APIRegistryDTO>>> GetAllAPIRegistriesByUserID(string id)
    {
        var registries = await _repository.GetAllByUserId(id);
        var mappedRegistries = registries.Select(r => new APIRegistryDTO
        {
            APIID = r.APIID,
            UserID = r.UserID,
            TargetURL = r.TargetURL,
            AuthType = r.AuthType,
            Status = r.Status.ToString(),
            VerificationToken = r.VerificationToken,
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt,
        });
        return Result<IEnumerable<APIRegistryDTO>>.Success(mappedRegistries);
    }

    public async Task<Result<APIRegistryDTO>> UpdateAPIRegistryAsync(Guid id, UpdateAPIRegistryDTO? registryDto)
    {
        try
        {
            if (registryDto == null) return Result<APIRegistryDTO>.BadRequest();
            var registry = await _repository.GetByIdAsync(id);

            if (registry == null)
                return Result<APIRegistryDTO>
                    .Failure(new Error(ErrorCodes.NotFound, $"API with the Id' {id} ' was not found"));

            if (registry.UserID != _userService.UserId)
                return Result<APIRegistryDTO>.Failure(
                    new Error(ErrorCodes.NotFound, $"API with the Id '{id}' was not found"));

            if (!UrlNormalizer.TryNormalize(registryDto.TargetURL, out var uri))
                return Result<APIRegistryDTO>.Failure(
                    new Error(ErrorCodes.Validation, "TargetURL must be an absolute http(s) URL with a domain name."));

            var canonical = UrlNormalizer.Canonical(uri);

            if (canonical != registry.TargetURL)
            {
                if (await APIRegistryExists(canonical))
                    return Result<APIRegistryDTO>.Failure(
                        new Error(ErrorCodes.Conflict, $"'{canonical}' is already registered."));

                registry.TargetURL = canonical;
                registry.VerificationToken = UrlNormalizer.NewVerificationToken();
                registry.VerifiedAt = null;
                registry.Status = APIStatus.Inactive;
            }

            registry.AuthType = registryDto.AuthType;
            registry.LastModifiedAt = DateTime.UtcNow;

            var newRegistry = await _repository.UpdateAsync(registry);

            return Result<APIRegistryDTO>.Success(new APIRegistryDTO
            {
                APIID = newRegistry.APIID,
                UserID = newRegistry.UserID,
                TargetURL = newRegistry.TargetURL,
                AuthType = newRegistry.AuthType,
                Status = newRegistry.Status.ToString(),
                VerificationToken = newRegistry.VerificationToken,
                CreatedAt = newRegistry.CreatedAt,
                LastModifiedAt = newRegistry.LastModifiedAt,
            });
        }
        catch (Exception)
        {
            return Result<APIRegistryDTO>.Failure();
        }
    }

    public async Task<Result<APIRegistryDTO>> CreateAPIRegistryAsync(CreateAPIRegistryDTO? registryDto)
    {
        try
        {
            var userId = _userService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<APIRegistryDTO>.Failure(new Error(ErrorCodes.Forbidden, "No authenticated user."));

            if (registryDto == null) return Result<APIRegistryDTO>.BadRequest();

            if (!UrlNormalizer.TryNormalize(registryDto.TargetURL, out var uri))
                return Result<APIRegistryDTO>.Failure(
                    new Error(ErrorCodes.Validation, "TargetURL must be an absolute http(s) URL with a domain name."));

            var canonical = UrlNormalizer.Canonical(uri);
            if (await APIRegistryExists(canonical))
                return Result<APIRegistryDTO>.Failure(
                    new Error(ErrorCodes.Conflict, $"'{canonical}' is already registered."));

            var registry = new APIRegistry
            {
                UserID = _userService.UserId,
                TargetURL = canonical,
                AuthType = registryDto.AuthType,
                Status = APIStatus.Inactive,
                VerificationToken = UrlNormalizer.NewVerificationToken(),
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };

            var createdRegistry = await _repository.AddAsync(registry);

            return Result<APIRegistryDTO>.Success(new APIRegistryDTO
            {
                APIID = createdRegistry.APIID,
                UserID = createdRegistry.UserID,
                TargetURL = createdRegistry.TargetURL,
                AuthType = createdRegistry.AuthType,
                Status = createdRegistry.Status.ToString(),
                VerificationToken = createdRegistry.VerificationToken,
                CreatedAt = createdRegistry.CreatedAt,
                LastModifiedAt = createdRegistry.LastModifiedAt,
            });
        }
        catch (Exception)
        {
            return Result<APIRegistryDTO>.Failure();
        }
    }

    public async Task<Result> DeleteAPIRegistryAsync(Guid id)
    {
            var registry = await _repository.GetByIdAsync(id);
            if (registry == null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"The API with ID '{id}' was not found"));

            if (registry.UserID != _userService.UserId)
                return Result.Failure(
                    new Error(ErrorCodes.NotFound, $"API with the Id '{id}' was not found"));
            await _repository.DeleteAsync(id);
            return Result.Success();
    }

    public async Task<bool> APIRegistryExists(string targetUrl)
    {
        if (string.IsNullOrWhiteSpace(targetUrl)) return false;
        string cleanedUrl = targetUrl.Trim();

        return await _repository
            .CheckExistsAsync(u => u.TargetURL == cleanedUrl);
    }
}