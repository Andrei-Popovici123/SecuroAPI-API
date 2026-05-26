using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IAPIRegistryService
{
    Task<Result<IEnumerable<APIRegistryDTO>>> GetAllAPIRegistriesAsync();
    Task<Result<APIRegistryDTO>> GetAPIRegistryByIdAsync(Guid id);
    Task<Result<APIRegistryDTO>> UpdateAPIRegistryAsync(Guid id, UpdateAPIRegistryDTO? registryDto);
    Task<Result<APIRegistryDTO>> CreateAPIRegistryAsync(CreateAPIRegistryDTO? registryDto);
    Task<Result> DeleteAPIRegistryAsync(Guid id);
    Task<bool> APIRegistryExists(string targetUrl);
}