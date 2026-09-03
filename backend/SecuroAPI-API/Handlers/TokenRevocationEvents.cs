using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using SecuroAPI.Common.Enums;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI_API.Handlers;

public class TokenRevocationEvents : JwtBearerEvents
{
    private readonly UserManager<ApplicationUser> _usersService;
    private readonly ILogger<TokenRevocationEvents> _logger;

    public TokenRevocationEvents(UserManager<ApplicationUser> usersService, ILogger<TokenRevocationEvents> logger)
    {
        _usersService = usersService;
        _logger = logger;
    }


    public override async Task TokenValidated(TokenValidatedContext ctx)
    {
        var userId = ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var stamp  = ctx.Principal?.FindFirst("sstamp")?.Value;

        if (userId is null || stamp is null)
        {
            ctx.Fail("Invalid token.");
            return;
        }

        var user = await _usersService.FindByIdAsync(userId);

        if (user is null || user.Status == UserStatus.Banned || user.SecurityStamp != stamp)
        {
            _logger.LogWarning("Rejected revoked token for {UserId}", userId);
            ctx.Fail("Token no longer valid.");
        }
    }
    
}