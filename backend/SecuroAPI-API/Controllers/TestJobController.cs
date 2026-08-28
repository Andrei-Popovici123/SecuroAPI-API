using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestConfig;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestJobController : BaseFunctionalController
{
    private readonly ITestJobService _testJobService;

    public TestJobController(ITestJobService testJobService)
    {
        _testJobService = testJobService;
    }
    [HttpPost("publish/{id:guid}")]
    public async Task<ActionResult<TestRunTriggeredDto>> Post(Guid id)
    {
       var testStatus=await _testJobService.RunTests(id);
        return ToActionResult(testStatus);
    }
}

