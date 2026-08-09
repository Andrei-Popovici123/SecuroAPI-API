using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface ITestRunService
{
    Task<Result<TestRunDto>> RunTests();
}