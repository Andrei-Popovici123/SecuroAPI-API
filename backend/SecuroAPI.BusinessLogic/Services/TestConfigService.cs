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

    public TestConfigService(ITestConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<TestConfigDto>>> GetAllTestConfigAsync()
    {
        var testConfig = await _repository.GetAllAsync();
        var mappedTestConfig = testConfig.Select(tc => new TestConfigDto()
        {
            ConfigId = tc.ConfigId,
            APIID = tc.APIID,
            EnabledTestIds = tc.EnabledTestIds,
        });

        return Result<IEnumerable<TestConfigDto>>.Success(mappedTestConfig);
    }

    public async Task<Result<TestConfigDto>> GetTestConfigByIdAsync(Guid id)
    {
        var testConfig = await _repository.GetByIdAsync(id);

        if (testConfig == null)
            return Result<TestConfigDto>
                .Failure(new Error(ErrorCodes.NotFound, $"TestConfig with the Id' {id} ' was not found"));

        return Result<TestConfigDto>.Success(new TestConfigDto
        {
            ConfigId = testConfig.ConfigId,
            APIID = testConfig.APIID,
            EnabledTestIds = testConfig.EnabledTestIds,
        });
    }

    public async Task<Result<TestConfigDto>> UpdateTestConfigAsync(Guid id, UpdateTestConfigDto? testConfigDto)
    {
        try
        {
            if (testConfigDto == null) return Result<TestConfigDto>.BadRequest();
            var testConfig = await _repository.GetByIdAsync(id);

            if (testConfig == null)
                return Result<TestConfigDto>
                    .Failure(new Error(ErrorCodes.NotFound, $"TestConfig with the Id' {id} ' was not found"));

            testConfig.EnabledTestIds = testConfigDto.EnabledTestIds;

            var updatedTestConfig = await _repository.UpdateAsync(testConfig);

            return Result<TestConfigDto>.Success(new TestConfigDto
            {
                ConfigId = updatedTestConfig.ConfigId,
                APIID = updatedTestConfig.APIID,
                EnabledTestIds = updatedTestConfig.EnabledTestIds
            });
        }
        catch (Exception)
        {
            return Result<TestConfigDto>.Failure();
        }
    }

    public async Task<Result<TestConfigDto>> CreateTestConfigAsync(CreateTestConfigDto? testConfigDto)
    {
        try
        {
            if (testConfigDto == null) return Result<TestConfigDto>.BadRequest();

            var testConfig = new TestConfig
            {
                APIID = testConfigDto.APIID,
                EnabledTestIds = testConfigDto.EnabledTestIds
            };

            var newTestConfig = await _repository.AddAsync(testConfig);


            return Result<TestConfigDto>.Success(new TestConfigDto
            {
                ConfigId = newTestConfig.ConfigId,
                APIID = newTestConfig.APIID,
                EnabledTestIds = newTestConfig.EnabledTestIds
            });
        }
        // catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        // {
        //     return Result<TestConfigDto>.Failure(new Error(ErrorCodes.NotFound,$"Validation failed: The provided APIID '{testConfigDto!.APIID}' does not exist."));
        // }
        // figure out how to get rid of 500 on wrong apiid
        catch (Exception)
        {
            return Result<TestConfigDto>.Failure();
        }
    }

    public async Task<Result> DeleteTestConfigAsync(Guid id)
    {
        try
        {
            var testConfig = await _repository.GetByIdAsync(id);
            if (testConfig == null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"TestConfig with ID '{id}' does not exist"));

            await _repository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    public async Task<Result<TestConfigDto>> GetTestConfigByAPIID (Guid id)
    {
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
}