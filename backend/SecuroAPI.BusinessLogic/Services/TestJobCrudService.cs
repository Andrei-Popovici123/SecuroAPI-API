using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class TestJobCrudService : ITestJobCrudService
{
    private readonly ITestJobRepository _repository;

    public TestJobCrudService(ITestJobRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<TestJobDto>>> GetAllTestJobsAsync()
    {
        var jobs = await _repository.GetAllAsync();
        return Result<IEnumerable<TestJobDto>>.Success(jobs.Select(MapToDto));
    }

    public async Task<Result<TestJobDto>> GetTestJobByIdAsync(Guid id)
    {
        var job = await _repository.GetByIdAsync(id);

        if (job == null)
            return Result<TestJobDto>
                .Failure(new Error(ErrorCodes.NotFound, $"TestJob with the Id '{id}' was not found"));

        return Result<TestJobDto>.Success(MapToDto(job));
    }

    public async Task<Result<IEnumerable<TestJobDto>>> GetAllTestJobsByAPIID(Guid apiId)
    {
        var jobs = await _repository.GetAllAsync(j => j.APIID == apiId);
        return Result<IEnumerable<TestJobDto>>.Success(jobs.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<TestJobDto>>> GetAllTestJobsByUserId(string userId)
    {
        var jobs = await _repository.GetAllAsync(j => j.UserId == userId);
        return Result<IEnumerable<TestJobDto>>.Success(jobs.Select(MapToDto));
    }

    public async Task<Result> DeleteTestJobAsync(Guid id)
    {
        try
        {
            var job = await _repository.GetByIdAsync(id);
            if (job == null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"TestJob with ID '{id}' does not exist"));

            if (job.Status is JobStatus.Queued or JobStatus.Running)
                return Result.BadRequest(new Error(ErrorCodes.Conflict,
                    $"Cannot delete a job that is {job.Status}"));

            await _repository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    private static TestJobDto MapToDto(TestJob job) => new()
    {
        JobId      = job.JobId,
        APIID      = job.APIID,
        Status     = job.Status,
        FailReason = job.FailReason,
        CreatedAt  = job.CreatedAt,
        FinishedAt = job.FinishedAt,
        UserId     = job.UserId,
        RatingId   = job.RatingId
    };
}
