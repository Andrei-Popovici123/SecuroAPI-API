using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleNames.Administrator)]
    public class AdminController : BaseFunctionalController
    {
        private readonly IAdministrationService _administrationService;
        private readonly IUserService _userService;

        public AdminController(IAdministrationService administrationService, IUserService userService)
        {
            _administrationService = administrationService;
            _userService = userService;
        }
        
        [HttpPost("registerAdmin")]
        public async Task<ActionResult<GetRegisteredUserDTO>> RegisterAdmin(RegisterUserDTO registerUserDto)
        {
            var result = await _userService.RegisterUserAsync(registerUserDto, RoleNames.Administrator);
            return ToActionResult(result);
        }
        
        [HttpPost("/users/{id}/approve")]
        public async Task<ActionResult<GetRegisteredUserDTO>> ApproveUser([FromRoute] string id)
        {
            var result = await _administrationService.ApproveUser(id);
            return ToActionResult(result);
        }
        [HttpPost("/users/{id}/reject")]
        public async Task<ActionResult<GetRegisteredUserDTO>> RejectUser([FromRoute] string id)
        {
            var result = await _administrationService.RejectUser(id);
            return ToActionResult(result);
        }
        [HttpPost("/API/{id:guid}/approve")]
        public async Task<ActionResult<GetRegisteredUserDTO>> ApproveAPI([FromRoute] Guid id)
        {
            var result = await _administrationService.ApproveAPI(id);
            return ToActionResult(result);
        }
        [HttpPost("/API/{id:guid}/Reject")]
        public async Task<ActionResult<GetRegisteredUserDTO>> RejectAPI([FromRoute] Guid id)
        {
            var result = await _administrationService.RejectAPI(id);
            return ToActionResult(result);
        }
    }
}
