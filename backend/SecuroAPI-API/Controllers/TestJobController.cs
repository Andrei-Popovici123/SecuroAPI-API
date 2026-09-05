using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestConfig;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApprovedUser")]
    public class TestJobController : BaseFunctionalController
    {
        private readonly ITestJobService _testJobService;
        private readonly ITestJobCrudService _testJobCrudService;

        public TestJobController(ITestJobService testJobService, ITestJobCrudService testJobCrudService)
        {
            _testJobService = testJobService;
            _testJobCrudService = testJobCrudService;
        }

        /// <summary>
        /// Triggers a scan. Jobs are created here only — there is no CRUD create,
        /// as that would bypass the authorization, config and cooldown gates.
        /// </summary>
        [HttpPost("publish/{id:guid}")]
        public async Task<ActionResult<TestRunTriggeredDto>> Post(Guid id)
        {
            var testStatus = await _testJobService.RunTests(id);
            return ToActionResult(testStatus);
        }

        /// <summary>
        /// Read/Delete operations for TestJob
        /// </summary>
        [HttpGet]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<IEnumerable<TestJobDto>>> Get()
        {
            var testJobs = await _testJobCrudService.GetAllTestJobsAsync();
            return ToActionResult(testJobs);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TestJobDto>> GetById(Guid id)
        {
            var testJob = await _testJobCrudService.GetTestJobByIdAsync(id);
            return ToActionResult(testJob);
        }

        [HttpGet("allByApiId/{id:guid}")]
        public async Task<ActionResult<IEnumerable<TestJobDto>>> GetByAPIID(Guid id)
        {
            var testJobs = await _testJobCrudService.GetAllTestJobsByAPIID(id);
            return ToActionResult(testJobs);
        }

        [HttpGet("myJobs")]
        public async Task<ActionResult<IEnumerable<TestJobDto>>> GetByUserId()
        {
            var testJobs = await _testJobCrudService.GetMyTestJobsAsync();
            return ToActionResult(testJobs);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedTestJob = await _testJobCrudService.DeleteTestJobAsync(id);
            return ToActionResult(deletedTestJob);
        }
    }
}

