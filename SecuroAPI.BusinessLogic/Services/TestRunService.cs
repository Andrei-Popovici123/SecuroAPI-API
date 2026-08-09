using Microsoft.AspNetCore.Http;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.BusinessLogic.Services.Publisher;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.Contracts.Events;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class TestRunService : ITestRunService
{
    private readonly ITestConfigRepository _configRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IAPIRegistryRepository _apiRegistryRepository;
    private readonly IScoreReportRepository _apiScoreReportRepository;
    private readonly IUserService _userService;
    private readonly ITestJobPublisher _publisher;
    
    public TestRunService(ITestConfigRepository configRepository, IRatingRepository ratingRepository, IAPIRegistryRepository apiRegistryRepository, IUserService userService, IScoreReportRepository apiScoreReportRepository, ITestJobPublisher publisher)
    {
        _configRepository = configRepository;
        _ratingRepository = ratingRepository;
        _apiRegistryRepository = apiRegistryRepository;
        _userService = userService;
        _apiScoreReportRepository = apiScoreReportRepository;
        _publisher = publisher;
    }
    
    public async Task<Result<TestRunDto>>RunTests()
    {
        // var userId = _userService.UserId;
        // var api = await _apiRegistryRepository.GetByIdAsync(APIID);
        //
        // if (api == null)
        // {
        //     return Result<TestRunDto>.NotFound(new Error(ErrorCodes.NotFound, $"API with ID '{APIID}' does not exist"));
        // }
        // if (userId !=api.UserID)
        // {
        //     return Result<TestRunDto>.BadRequest(new Error(ErrorCodes.Forbidden, $"Unauthorized to run tests on this API"));
        //     
        // }
        // var testConfig = await _configRepository.GetByAPIID(APIID);
        // if ( testConfig  == null)
        // {
        //     return Result<TestRunDto>.NotFound(new Error(ErrorCodes.NotFound, $"TestConfiguration for Api with ID '{APIID}' does not exist"));
        // }
        // return new Result<TestRunDto>();

        await _publisher.PublishAsync(new TestJobMessage("https://x"));
        return  Result<TestRunDto>.Success(new TestRunDto());
    }
}