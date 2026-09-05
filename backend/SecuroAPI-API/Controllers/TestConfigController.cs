using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.TestConfig;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApprovedUser")]
    public class TestConfigController : BaseFunctionalController
    {
        private readonly ITestConfigService _testConfigService;

        public TestConfigController(ITestConfigService testConfigService)
        {
            _testConfigService = testConfigService;
        }

        /// <summary>
        /// Crud Operations for TestConfig
        /// </summary>
        /// <returns></returns>
        ///
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestConfigDto>>> Get()
        {
            var testConfigs = await _testConfigService.GetAllTestConfigAsync();
            return ToActionResult(testConfigs);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TestConfigDto>> GetById(Guid id)
        {
            var testConfig = await _testConfigService.GetTestConfigByIdAsync(id);
            return ToActionResult(testConfig);
        }


        [HttpGet("byApiId/{id:guid}")]
        public async Task<ActionResult<TestConfigDto>> GetByAPIID(Guid id)
        {
            var testConfig = await _testConfigService.GetTestConfigByAPIID(id);
            return ToActionResult(testConfig);
        }

        [HttpPost]
        public async Task<ActionResult<TestConfigDto>> Post([FromBody] CreateTestConfigDto testConfigDto)
        {
            var newTestConfigResult = await _testConfigService.CreateTestConfigAsync(testConfigDto);
            
            if (!newTestConfigResult.IsSuccess) return MapErrorToResponse(newTestConfigResult.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = newTestConfigResult.Value!.ConfigId },
                newTestConfigResult.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<TestConfigDto>> Put(Guid id, [FromBody] UpdateTestConfigDto testConfigDto)
        {
            var registry = await _testConfigService.UpdateTestConfigAsync(id, testConfigDto);
            return ToActionResult(registry);
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedTestConfig = await _testConfigService.DeleteTestConfigAsync(id);
            return ToActionResult(deletedTestConfig);
        }
    }
}