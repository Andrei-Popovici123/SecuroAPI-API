using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseFunctionalController
    {

        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<GetRegisteredUserDTO>> RegisterUser(RegisterUserDTO registerUserDto)
        {
            var result = await _userService.RegisterUserAsync(registerUserDto, RoleNames.User,UserStatus.New);
            return ToActionResult(result);
        }
        
        
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginUserDTO loginUserDto)
        {
            var result = await _userService.LoginUserAsync(loginUserDto);
            return ToActionResult(result);
        }
    }
}