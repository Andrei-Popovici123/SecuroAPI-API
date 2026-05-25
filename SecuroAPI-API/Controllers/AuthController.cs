using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI_API.Constants;
using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.Results;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : BaseFunctionalController
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUserDto)
        {
            var user = new ApplicationUser
            {
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                UserName = registerUserDto.Email,
            };

            var createdUser = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!createdUser.Succeeded)
            {
                var errors = createdUser.Errors
                    .Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
                return MapErrorToResponse(errors);
            }

            //to add additional functionality
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO loginUserDto)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null) return Unauthorized(new { message = "Invalid Credentials" });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!isPasswordValid) return Unauthorized(new { message = "Invalid Credentials" });

            return Ok();
        }
    }
}