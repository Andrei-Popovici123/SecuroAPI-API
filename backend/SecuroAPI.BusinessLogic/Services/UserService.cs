using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Models;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SecuroAPI.BusinessLogic.Services;


public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    //this might be fix, gotta check if it broke something
    public string UserId => _httpContextAccessor?
        .HttpContext?
        .User?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty;

    public UserService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtOptions, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions;
        _httpContextAccessor = httpContextAccessor;
    }



    public async Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto, string role,UserStatus status)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email,
            Status = status
        };
        
        

        var createdUser = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!createdUser.Succeeded)
        {
            var errors = createdUser.Errors
                .Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<GetRegisteredUserDTO>.BadRequest(errors);
        }

        await _userManager.AddToRoleAsync(user, role);

        var registeredUser = new GetRegisteredUserDTO
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            Id = user.Id,
            Status = user.Status.ToString()
        };
        //to add additional functionality
        return Result<GetRegisteredUserDTO>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginUserAsync(LoginUserDTO loginUserDto)
    {
        var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
        if (user == null)
            return Result<string>
                .Failure(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid)
            return Result<string>
                .Failure(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));

        // token Issuing
        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    public async Task<Result<UserStatus>> GetStatusAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<UserStatus>.NotFound(new Error(ErrorCodes.NotFound, $"User '{userId}' not found"));

        return Result<UserStatus>.Success(user.Status);
    }

    public async Task<Result<GetRegisteredUserDTO>> GetByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<GetRegisteredUserDTO>.NotFound(new Error(ErrorCodes.NotFound, $"User '{userId}' not found"));

        var userDto = new GetRegisteredUserDTO
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status.ToString()
        };
        return Result<GetRegisteredUserDTO>.Success(userDto);
    }

    public async Task<Result<IEnumerable<GetRegisteredUserDTO>>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        var usersDto = users.Select(user => new GetRegisteredUserDTO
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status.ToString()
        });
        return Result<IEnumerable<GetRegisteredUserDTO>>.Success(usersDto);
    }
    private async Task<string> GenerateToken(ApplicationUser user)
    {
        //basic user claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
        };

        //user role claims
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();
        //Jwt Key credentials

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //Create and encode token
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Value.Issuer,
            audience: _jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtOptions.Value.DurationInMinutes)),
            signingCredentials: credentials
        );

        //return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}