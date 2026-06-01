using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Handlers;

using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess.Entities;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyValidatorService _apiKeyValidatorService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey = string.Empty;

        if (Request.Headers.TryGetValue(AuthenticationDefaults.ApiHeaderName, out var headerValues))
        {
            apiKey = headerValues.ToString();
        }

        if (string.IsNullOrWhiteSpace(apiKey)) return AuthenticateResult.NoResult();

        var valid = await _apiKeyValidatorService.IsValidAsync(apiKey, Context.RequestAborted);
        if (!valid) return AuthenticateResult.Fail("Invalid API key");
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier,"apiKey"),
            new(ClaimTypes.Name,"ApiKeyClient"),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}