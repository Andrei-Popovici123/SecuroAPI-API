using Microsoft.Extensions.Configuration;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class ApiKeyValidatorService(IConfiguration configuration) : IApiKeyValidatorService
{
    //to add a repository
    // querry api key table to chec for validity and expiration
    
    /// <summary>
    /// Might drop api keys entirely in favor of rabbit mq
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    
    public Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
    {
        return Task.FromResult(apiKey.Equals(configuration["ApiKey"]));
    }
}