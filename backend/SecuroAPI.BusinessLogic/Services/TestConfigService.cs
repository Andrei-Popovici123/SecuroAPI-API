using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecuroAPI.BusinessLogic.DTO_s.TestConfig;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class TestConfigService : ITestConfigService
{
    private readonly ITestConfigRepository _repository;
    private readonly IAPIRegistryRepository _apiRepository;
    private readonly IUserService _userService;

    public TestConfigService(ITestConfigRepository repository, IAPIRegistryRepository apiRepository,
        IUserService userService)
    {
        _repository = repository;
        _apiRepository = apiRepository;
        _userService = userService;
    }

    public async Task<Result<IEnumerable<TestConfigDto>>> GetAllTestConfigAsync()
    {
        var testConfig = await _repository.GetAllAsync();
        var mappedTestConfig = testConfig.Select(MapToDto);

        return Result<IEnumerable<TestConfigDto>>.Success(mappedTestConfig);
    }

    public async Task<Result<TestConfigDto>> GetTestConfigByIdAsync(Guid id)
    {
        var testConfig = await _repository.GetByIdAsync(id);

        if (testConfig == null || !await OwnsApi(testConfig.APIID))
            return Result<TestConfigDto>
                .Failure(new Error(ErrorCodes.NotFound, $"TestConfig with the Id' {id} ' was not found"));

        return Result<TestConfigDto>.Success(MapToDto(testConfig));
    }

    public async Task<Result<TestConfigDto>> UpdateTestConfigAsync(Guid id, UpdateTestConfigDto? testConfigDto)
    {

            if (testConfigDto == null) return Result<TestConfigDto>.BadRequest();
            var testConfig = await _repository.GetByIdAsync(id);

            if (testConfig == null)
                return Result<TestConfigDto>
                    .Failure(new Error(ErrorCodes.NotFound, $"TestConfig with the Id' {id} ' was not found"));

            if (!await OwnsApi(testConfig.APIID))
                return Result<TestConfigDto>.Failure(
                    new Error(ErrorCodes.NotFound, $"TestConfig with the Id '{id}' was not found"));

            var idCheck = ValidateEnabledIds(testConfigDto.EnabledTestIds);
            if (!idCheck.IsSuccess)
                return Result<TestConfigDto>.Failure(idCheck.Errors);
            
            testConfig.EnabledTestIds = testConfigDto.EnabledTestIds;

            var updatedTestConfig = await _repository.UpdateAsync(testConfig);

            return Result<TestConfigDto>.Success(MapToDto(updatedTestConfig));

    }

    public async Task<Result<TestConfigDto>> CreateTestConfigAsync(CreateTestConfigDto? testConfigDto)
    {

            if (testConfigDto == null) return Result<TestConfigDto>.BadRequest();
            
            if (await _repository.CheckExistsAsync(tc => tc.APIID == testConfigDto.APIID))
                return Result<TestConfigDto>.Failure(
                    new Error(ErrorCodes.Conflict, "A configuration already exists for this API."));
            
            if (!await OwnsApi(testConfigDto.APIID))
                return Result<TestConfigDto>.Failure(
                    new Error(ErrorCodes.NotFound, $"API with the Id '{testConfigDto.APIID}' was not found"));

            var idCheck = ValidateEnabledIds(testConfigDto.EnabledTestIds);
            if (!idCheck.IsSuccess)
                return Result<TestConfigDto>.Failure(idCheck.Errors); 
            
            var testConfig = new TestConfig
            {
                APIID = testConfigDto.APIID,
                EnabledTestIds = testConfigDto.EnabledTestIds
            };

            var newTestConfig = await _repository.AddAsync(testConfig);


            return Result<TestConfigDto>.Success(MapToDto(newTestConfig));
    }

    public async Task<Result> DeleteTestConfigAsync(Guid id)
    {
        var testConfig = await _repository.GetByIdAsync(id);
        if (testConfig == null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"TestConfig with ID '{id}' does not exist"));

        await _repository.DeleteAsync(id);
        return Result.Success();
    }

    public async Task<Result<TestConfigDto>> GetTestConfigByAPIID(Guid id)
    {
        if (!await OwnsApi(id))
            return Result<TestConfigDto>.Failure(
                new Error(ErrorCodes.NotFound, $"TestConfig for the API with id '{id}' was not found"));
        var testConfig = await _repository.GetByAPIID(id);

        if (testConfig == null)
            return Result<TestConfigDto>
                .Failure(new Error(ErrorCodes.NotFound, $"TestConfig for the API with id' {id} ' was not found"));


        var mappedTestConfig = new TestConfigDto()
        {
            ConfigId = testConfig.ConfigId,
            APIID = testConfig.APIID,
            EnabledTestIds = testConfig.EnabledTestIds,
        };

        return Result<TestConfigDto>.Success(mappedTestConfig);
    }

    private async Task<bool> OwnsApi(Guid apiId)
        => await _apiRepository.CheckExistsAsync(a => a.APIID == apiId && a.UserID == _userService.UserId);

    private static TestConfigDto MapToDto(TestConfig tc) => new()
    {
        ConfigId = tc.ConfigId,
        APIID = tc.APIID,
        EnabledTestIds = tc.EnabledTestIds,
    };
    
    private static Result ValidateEnabledIds(IReadOnlyList<Guid> ids)
    {
        if (ids.Count == 0)
            return Result.BadRequest(new Error(ErrorCodes.Validation,
                "At least one test must be enabled."));

        var unknown = ids.Where(id => !TestCatalog.ById.ContainsKey(id)).ToList();
        if (unknown.Count > 0)
            return Result.BadRequest(new Error(ErrorCodes.Validation,
                $"Unknown test ids: {string.Join(", ", unknown)}"));

        return Result.Success();
    }
}