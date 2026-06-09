using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class APIRegistryController : BaseFunctionalController
    {
        private readonly IAPIRegistryService _apiRegistryService;

        public APIRegistryController(IAPIRegistryService apiRegistryService)
        {
            _apiRegistryService = apiRegistryService;
        }

        /// <summary>
        /// Crud Operations for APIRegistry
        /// </summary>
        /// <returns></returns>
        ///
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<APIRegistryDTO>>> Get()
        {
            var registries = await _apiRegistryService.GetAllAPIRegistriesAsync();
            return ToActionResult(registries);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<APIRegistryDTO>> GetById(Guid id)
        {
            var registry = await _apiRegistryService.GetAPIRegistryByIdAsync(id);
            return ToActionResult(registry);
        }

        
        [HttpGet("userId/{id}")]
        
        public async Task<ActionResult<IEnumerable<APIRegistryDTO>>> GetByUserId(string id)
        {
            var registries = await _apiRegistryService.GetAllAPIRegistriesByUserID(id);
            return ToActionResult(registries);
        }
        
        [HttpPost]
        public async Task<ActionResult<APIRegistryDTO>> Post([FromBody] CreateAPIRegistryDTO registryDto)
        {
            var newApiRegistryResult = await _apiRegistryService.CreateAPIRegistryAsync(registryDto);
            if (!newApiRegistryResult.IsSuccess) return MapErrorToResponse(newApiRegistryResult.Errors);
            return CreatedAtAction(nameof(GetById), new { id = newApiRegistryResult.Value!.APIID },
                newApiRegistryResult.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<APIRegistryDTO>> Put(Guid id, [FromBody] UpdateAPIRegistryDTO registryDto)
        {
            var registry = await _apiRegistryService.UpdateAPIRegistryAsync(id, registryDto);
            return ToActionResult(registry);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedRegistry = await _apiRegistryService.DeleteAPIRegistryAsync(id);
            return ToActionResult(deletedRegistry);
        }
    }
}