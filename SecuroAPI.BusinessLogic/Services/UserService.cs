using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecuroAPI.BusinessLogic.Constants;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Results;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SecuroAPI.BusinessLogic.Services;

public class UserService: IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
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
        
        // token Issuing
        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        //basic user claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),

        };
        
        //user role claims
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();
        //Jwt Key credentials

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //Create and encode token
        var token = new JwtSecurityToken(
            issuer:_configuration["JwtSettings:Issuer"],
            audience:_configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentials
            );

        //return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}