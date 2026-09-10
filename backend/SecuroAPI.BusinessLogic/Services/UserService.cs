using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<UserService> _logger;

    public string UserId => _httpContextAccessor?
        .HttpContext?
        .User?
        .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty;

    public bool IsAdministrator => _httpContextAccessor?.HttpContext?
        .User?.IsInRole(RoleNames.Administrator) ?? false;

    public UserService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtOptions,
        IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions;
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
        _logger = logger;
    }


    public async Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto, string role,
        UserStatus status)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email,
            CompanyName = registerUserDto.CompanyName,
            Status = status
        };

        var createdUser = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!createdUser.Succeeded)
        {
            var errors = createdUser.Errors
                .Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<GetRegisteredUserDTO>.BadRequest(errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return Result<GetRegisteredUserDTO>.Failure(
                new Error(ErrorCodes.Failure, "Registration failed."));
        }

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

        var signIn = await _signInManager.CheckPasswordSignInAsync(
            user, loginUserDto.Password, lockoutOnFailure: true);

        if (signIn.IsLockedOut)
            return Result<string>.Failure(
                new Error(ErrorCodes.Forbidden, "Account temporarily locked. Try again later."));

        if (!signIn.Succeeded)
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials."));

        if (user.Status == UserStatus.Banned)
            return Result<string>.Failure(new Error(ErrorCodes.Forbidden, "Account suspended."));

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        // token Issuing
        var token = await GenerateToken(user);
        _logger.LogWarning("User {UserId} has successfully logged in from {IP}",
            loginUserDto.Email, _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress);
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
            CompanyName = user.CompanyName,
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
            CompanyName = user.CompanyName,
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
            new Claim("sstamp", user.SecurityStamp ?? string.Empty),
            new Claim("status", user.Status.ToString())
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