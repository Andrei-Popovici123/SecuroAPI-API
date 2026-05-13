using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIRegistryController : ControllerBase
    {
        private IAPIRegistryService _apiRegistryService;

        public APIRegistryController(IAPIRegistryService apiRegistryService)
        {
            _apiRegistryService = apiRegistryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var registries = await _apiRegistryService.GetAllAPIRegistriesAsync();
             return Ok(registries);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var registry = await _apiRegistryService.GetAPIRegistryByIdAsync(id);
            if (registry == null) return NotFound($"API Registry with ID {id} was not found.");
            return Ok(registry);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateAPIRegistryDTO registryDto)
        {
            if (registryDto == null) return BadRequest("API Registry data is null");
            var newAPIRegistry = await _apiRegistryService.CreateAPIRegistruAsync(registryDto);
            return Ok(newAPIRegistry);

        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateAPIRegistryDTO registryDto)
        {
            if(registryDto == null )return BadRequest("API Registry data is null");
            var registry = await _apiRegistryService.UpdateAPIRegistryAsync(id, registryDto);
            if (registry == null) return NotFound($"API Registry with ID {id} was not found.");
            return Ok(registry);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var registry = await _apiRegistryService.GetAPIRegistryByIdAsync(id);
            
            if (registry == null) return NotFound($"API Registry with ID {id} was not found.");
            
            await _apiRegistryService.DeleteAPIRegistryAsync(id);
            return NoContent();
        }
    }
}
