namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IApiKeyValidatorService
{
    Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default);
}