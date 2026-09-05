using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface ITestJobCrudService
{
        Task<Result<IEnumerable<TestJobDto>>> GetMyTestJobsAsync();
        Task<Result<IEnumerable<TestJobDto>>> GetAllTestJobsAsync();
        Task<Result<TestJobDto>> GetTestJobByIdAsync(Guid id);
        Task<Result<IEnumerable<TestJobDto>>> GetAllTestJobsByAPIID(Guid apiId);
        Task<Result> DeleteTestJobAsync(Guid id);
    
}