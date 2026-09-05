using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleNames.Administrator, Policy = "ApprovedUser")]
    public class AdminController : BaseFunctionalController
    {
        private readonly IAdministrationService _administrationService;
        private readonly IUserService _userService;

        public AdminController(IAdministrationService administrationService, IUserService userService)
        {
            _administrationService = administrationService;
            _userService = userService;
        }
        
        [HttpGet("apis/pending")]
        public async Task<ActionResult<IEnumerable<APIRegistryDTO>>> GetUnapprovedAPIs()
        {
            var registries = await _administrationService.GetAllUnapprovedAPIs();
            return ToActionResult(registries);
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<GetRegisteredUserDTO>>> GetUsers()
        {
            var users = await _administrationService.GetAllUsers();
            return ToActionResult(users);
        }

        [HttpGet("users/pending")]
        public async Task<ActionResult<IEnumerable<GetRegisteredUserDTO>>> GetUnapprovedUsers()
        {
            var registries = await _administrationService.GetAllUnapprovedUsers();
            return ToActionResult(registries);
        }

        [HttpPost("users/{id}/approve")]
        public async Task<ActionResult<GetRegisteredUserDTO>> ApproveUser([FromRoute] string id)
        {
            var result = await _administrationService.ApproveUser(id);
            return ToActionResult(result);
        }

        [HttpPost("users/{id}/reject")]
        public async Task<ActionResult<GetRegisteredUserDTO>> RejectUser([FromRoute] string id)
        {
            var result = await _administrationService.RejectUser(id);
            return ToActionResult(result);
        }

        [HttpPost("apis/{id:guid}/approve")]
        public async Task<ActionResult<GetRegisteredUserDTO>> ApproveAPI([FromRoute] Guid id)
        {
            var result = await _administrationService.ApproveAPI(id);
            return ToActionResult(result);
        }

        [HttpPost("apis/{id:guid}/Reject")]
        public async Task<ActionResult<GetRegisteredUserDTO>> RejectAPI([FromRoute] Guid id)
        {
            var result = await _administrationService.RejectAPI(id);
            return ToActionResult(result);
        }
    }
}