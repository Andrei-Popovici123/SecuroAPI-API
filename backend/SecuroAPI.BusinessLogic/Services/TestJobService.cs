using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.BusinessLogic.Services.Publisher;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.Contracts.Events;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class TestJobService : ITestJobService
{
    private readonly ITestConfigRepository _configRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IAPIRegistryRepository _apiRegistryRepository;
    private readonly IScoreReportRepository _apiScoreReportRepository;
    private readonly IUserService _userService;
    private readonly ITestJobPublisher _publisher;
    private readonly ITestJobRepository _jobRepository;
    private readonly ILogger<TestJobService> _logger;
    private static readonly TimeSpan ScanJobCooldown = TimeSpan.FromSeconds(120);

    private static readonly JsonSerializerOptions ReportOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public TestJobService(ITestConfigRepository configRepository, IRatingRepository ratingRepository,
        IAPIRegistryRepository apiRegistryRepository, IUserService userService,
        IScoreReportRepository apiScoreReportRepository, ITestJobPublisher publisher,
        ITestJobRepository jobRepository, ILogger<TestJobService> logger)
    {
        _configRepository = configRepository;
        _ratingRepository = ratingRepository;
        _apiRegistryRepository = apiRegistryRepository;
        _userService = userService;
        _apiScoreReportRepository = apiScoreReportRepository;
        _publisher = publisher;
        _jobRepository = jobRepository;
        _logger = logger;
    }

//possible race condition for a double request. Will be investigated at another time
    public async Task<Result<TestRunTriggeredDto>> RunTests(Guid apiId)
    {
        var authResult = await AuthorizeScanAsync(apiId);
        if (!authResult.IsSuccess)
            return Result<TestRunTriggeredDto>.Failure(authResult.Errors);

        var api = authResult.Value!;

        var configResult = await ValidateConfigAsync(apiId);
        if (!configResult.IsSuccess)
            return Result<TestRunTriggeredDto>.Failure(configResult.Errors);

        var gateResult = await CheckJobGatesAsync(apiId);
        if (!gateResult.IsSuccess)
            return Result<TestRunTriggeredDto>.Failure(gateResult.Errors);

        return await QueueJobAsync(api, configResult.Value!);
    }


    private async Task<Result<APIRegistry>> AuthorizeScanAsync(Guid apiId)
    {
        var api = await _apiRegistryRepository.GetByIdAsync(apiId);
        if (api == null)
            return Result<APIRegistry>.NotFound(
                new Error(ErrorCodes.NotFound, $"API with ID '{apiId}' does not exist"));

        if (_userService.UserId != api.UserID)
            return Result<APIRegistry>.Forbidden(
                new Error(ErrorCodes.Forbidden, "Unauthorized to run tests on this API"));

        var userStatus = await _userService.GetStatusAsync(_userService.UserId);
        if (!userStatus.IsSuccess || userStatus.Value != UserStatus.Approved)
            return Result<APIRegistry>.Forbidden(
                new Error(ErrorCodes.Forbidden, "Unauthorized to run tests"));

        if (api.Status != APIStatus.Approved)
            return Result<APIRegistry>.BadRequest(
                new Error(ErrorCodes.Forbidden, $"API is '{api.Status}'; must be Approved"));

        return Result<APIRegistry>.Success(api);
    }

    private async Task<Result<TestConfig>> ValidateConfigAsync(Guid apiId)
    {
        var config = await _configRepository.GetByAPIID(apiId);
        return config == null || config.EnabledTestIds.Count == 0
            ? Result<TestConfig>.BadRequest(
                new Error(ErrorCodes.Validation, "No tests enabled for this API"))
            : Result<TestConfig>.Success(config);
    }

    private async Task<Result> CheckJobGatesAsync(Guid apiId)
    {
        var latestJob = await _jobRepository.GetLatestForApiAsync(apiId);
        if (latestJob == null) return Result.Success();

        if (latestJob.Status is JobStatus.Queued or JobStatus.Running)
            return Result.BadRequest(new Error(ErrorCodes.BadRequest,
                $"A scan is already {latestJob.Status} for this API"));


        var timeElapsedSinceLastJob = DateTime.UtcNow - (latestJob.FinishedAt ?? latestJob.CreatedAt);
        if (timeElapsedSinceLastJob < ScanJobCooldown && latestJob.Status == JobStatus.Completed)
            return Result.BadRequest(new Error(ErrorCodes.BadRequest,
                $"Please wait {(ScanJobCooldown - timeElapsedSinceLastJob).TotalSeconds:F0}s before scanning again"));

        return Result.Success();
    }

    private async Task<Result<TestRunTriggeredDto>> QueueJobAsync(APIRegistry api, TestConfig config)
    {
        var jobId = Guid.NewGuid();

        var job = await _jobRepository.AddAsync(new TestJob
        {
            JobId = jobId,
            APIID = api.APIID,
            UserId = _userService.UserId,
            Status = JobStatus.Queued,
            CreatedAt = DateTime.UtcNow
        });

        try
        {
            await _publisher.PublishAsync(
                new TestJobMessage(jobId, api.APIID, api.TargetURL, config.EnabledTestIds));
        }
        catch (Exception)
        {
            job.Status = JobStatus.Failed;
            job.FinishedAt = DateTime.UtcNow;
            await _jobRepository.UpdateAsync(job);

            return Result<TestRunTriggeredDto>.Failure(
                new Error(ErrorCodes.Failure, "Failed to queue scan; please retry"));
        }

        return Result<TestRunTriggeredDto>.Success(new TestRunTriggeredDto
        {
            JobId = jobId,
            APIID = api.APIID,
            JobStatus = JobStatus.Queued.ToString()
        });
    }

    public async Task<Result> ApplyStatusAsync(JobStatusDto jobStatusDto)
    {
        var job = await _jobRepository.GetByIdAsync(jobStatusDto.JobId);

        if (job is null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Job '{jobStatusDto.JobId}' does not exist"));

        if (job.Status is JobStatus.Completed or JobStatus.Failed)
            return Result.BadRequest(new Error(ErrorCodes.Conflict,
                $"Job '{jobStatusDto.JobId}' is already {job.Status}"));

        job.Status = jobStatusDto.Status;

        if (jobStatusDto.Status is JobStatus.Completed or JobStatus.Failed)
        {
            job.FinishedAt = jobStatusDto.OccurredAt.ToUniversalTime();
            job.FailReason = jobStatusDto.FailReason;
        }

        await _jobRepository.UpdateAsync(job);
        return Result.Success();
    }

    public async Task<Result<int>> ClearStuckJobsAsync(TimeSpan stuckAfter)
    {
        var cutoff = DateTime.UtcNow - stuckAfter;

        var stale = await _jobRepository.GetAllAsync(j =>
            (j.Status == JobStatus.Running || j.Status == JobStatus.Queued)
            && j.CreatedAt < cutoff);

        var cleared = 0;
        foreach (var job in stale)
        {
            var result =
                await ApplyStatusAsync(new JobStatusDto(job.JobId, JobStatus.Failed, DateTime.UtcNow, "Job Timed out"));
            if (result.IsSuccess)
            {
                cleared++;
                _logger.LogWarning("Job {JobId} stuck since {CreatedAt} → Failed", job.JobId, job.CreatedAt);
            }
        }

        return Result<int>.Success(cleared);
    }

    public async Task<Result> SaveResultAsync(JobResultDto jobResultDto)
    {
        var job = await _jobRepository.GetByIdAsync(jobResultDto.JobId);
        if (job is null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Job '{jobResultDto.JobId}' does not exist"));

        if (job.Status is JobStatus.Completed or JobStatus.Failed)
            return Result.BadRequest(new Error(ErrorCodes.Conflict,
                $"Job '{jobResultDto.JobId}' is already {job.Status}"));

        if (jobResultDto.ExitCode != 0)
        {
            _logger.LogWarning("Job {JobId} runner exited {ExitCode}", jobResultDto.JobId, jobResultDto.ExitCode);
            return await FailJobAsync(jobResultDto.JobId, $"Job exited with code {jobResultDto.ExitCode}");
        }

        ScanReport? report;
        try
        {
            report = JsonSerializer.Deserialize<ScanReport>(jobResultDto.Output, ReportOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Job {JobId} produced unparseable output", jobResultDto.JobId);
            return await FailJobAsync(jobResultDto.JobId, "The Job returned an invalid output format");
        }

        if (report is null)
            return await FailJobAsync(jobResultDto.JobId, "Runner produced empty output");

        var (vulnerabilityScore, overallScore) = CalculateScore(report.Findings);

        var rating = await _ratingRepository.AddAsync(new Rating
        {
            RatingId = Guid.NewGuid(),
            APIID = jobResultDto.APIID,
            NumberOfTests = report.ChecksRun.Count,
            VulnerabilityScore = vulnerabilityScore,
            OverallScore = overallScore,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        });

        foreach (var finding in report.Findings)
        {
            await _apiScoreReportRepository.AddAsync(new ScoreReport
            {
                ReportId = Guid.NewGuid(),
                RatingId = rating.RatingId,
                Severity = finding.Severity,
                Summary = finding.Summary,
                Recommendation = finding.Recommendation,
                FinishedAt = report.FinishedAt.ToUniversalTime(),
                Check = finding.Check,
                Evidence = finding.Evidence is null ? null
                    : $"{finding.Evidence.Url} — {finding.Evidence.Indicator}"
            });
        }

        job.RatingId = rating.RatingId;
        job.Status = JobStatus.Completed;
        job.FinishedAt = report.FinishedAt.ToUniversalTime();
        await _jobRepository.UpdateAsync(job);

        _logger.LogInformation("Job {JobId} findings {Findings}, score {Score}",
            jobResultDto.JobId, report.Findings.Count, overallScore);

        return Result.Success();
    }

    private (int vulnerabilityScore, int overallScore) CalculateScore(IReadOnlyList<ScanFinding> reportFindings)
    {
        return (1, 1);
    }

    private Task<Result> FailJobAsync(Guid jobId, string reason) =>
        ApplyStatusAsync(new JobStatusDto(jobId, JobStatus.Failed, DateTime.UtcNow, reason));
}