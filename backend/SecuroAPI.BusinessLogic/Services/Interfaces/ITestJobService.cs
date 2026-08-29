using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.Contracts.Events;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface ITestJobService
{
    Task<Result<TestRunTriggeredDto>> RunTests(Guid APIID);

    Task<Result> ApplyStatusAsync(JobStatusDto jobStatusDto);

    Task<Result<int>> ClearStuckJobsAsync(TimeSpan stuckAfter);

    Task<Result> PersistResultAsync(JobResultDto jobResultDto);
}