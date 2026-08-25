using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestConfig;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestRunController : BaseFunctionalController
{
    private readonly ITestRunService _testRunService;

    public TestRunController(ITestRunService testRunService)
    {
        _testRunService = testRunService;
    }
    [HttpPost("publish/{id:guid}")]
    public async Task<ActionResult<TestRunDto>> Post(Guid id)
    {
       await _testRunService.RunTests(id);
        return Ok();
    }
}

