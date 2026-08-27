using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface ITestRunService
{
    Task<Result<TestRunTriggeredDto>> RunTests(Guid APIID);
}