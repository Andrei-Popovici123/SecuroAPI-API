using SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;
using SecuroAPI.BusinessLogic.DTO_s.TestConfig;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface ITestConfigService
{
    Task<Result<IEnumerable<TestConfigDto>>> GetAllTestConfigAsync();
    Task<Result<TestConfigDto>> GetTestConfigByIdAsync(Guid id);
    Task<Result<TestConfigDto>> UpdateTestConfigAsync(Guid id, UpdateTestConfigDto? testConfigDto);
    Task<Result<TestConfigDto>> CreateTestConfigAsync(CreateTestConfigDto? testConfigDto);
    Task<Result> DeleteTestConfigAsync(Guid id);
    Task<Result<TestConfigDto>> GetTestConfigByAPIID(Guid id);
}