using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApprovedUser")]
    public class APIRegistryController : BaseFunctionalController
    {
        private readonly IAPIRegistryService _apiRegistryService;
        private readonly IVerificationService _verificationService;

        public APIRegistryController(IAPIRegistryService apiRegistryService, IVerificationService verificationService)
        {
            _apiRegistryService = apiRegistryService;
            _verificationService = verificationService;
        }

        /// <summary>
        /// Crud Operations for APIRegistry
        /// </summary>
        /// <returns></returns>
        ///
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<APIRegistryDTO>> GetById(Guid id)
        {
            var registry = await _apiRegistryService.GetAPIRegistryByIdAsync(id);
            return ToActionResult(registry);
        }

        [Authorize(Roles = RoleNames.Administrator)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<APIRegistryDTO>>> Get()
        {
            return ToActionResult(await _apiRegistryService.GetAllAPIRegistriesAsync());
        }

        [HttpGet("myApis")]
        
        public async Task<ActionResult<IEnumerable<APIRegistryDTO>>> GetMyApis()
        {
            var registries = await _apiRegistryService.GetMyRegistriesAsync();
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


        [HttpPost("{id:guid}/verify")]
        public async Task<ActionResult<VerificationStatusDTO>> Verify(Guid id, CancellationToken ct)
        {
            return ToActionResult(await _verificationService.VerifyAsync(id, ct));
        }
    }
}