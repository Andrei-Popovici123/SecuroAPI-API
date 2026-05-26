using Microsoft.AspNetCore.Identity;
using SecuroAPI.BusinessLogic.Constants;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Results;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.BusinessLogic.Services;

public class UserService: IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto)
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
            return Result<GetRegisteredUserDTO>.BadRequest(errors);
        }

        var registeredUser = new GetRegisteredUserDTO
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            Id = user.Id
        };
        //to add additional functionality
        return Result<GetRegisteredUserDTO>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginUserAsync(LoginUserDTO loginUserDto)
    {
        var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
        if (user == null) return Result<string>
            .Failure(new Error( ErrorCodes.BadRequest, "Invalid Credentials" ));

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid) return Result<string>
            .Failure(new Error( ErrorCodes.BadRequest, "Invalid Credentials" ));

        return Result<string>.Success("Login Successful");
    }
}