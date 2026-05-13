using SecuroAPI.BusinessLogic.DTO_s;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IAPIRegistryService
{
    Task<IEnumerable<APIRegistryDTO>> GetAllAPIRegistriesAsync();
    Task<APIRegistryDTO?> GetAPIRegistryByIdAsync(Guid id);
    Task <APIRegistryDTO?> UpdateAPIRegistryAsync(Guid id, UpdateAPIRegistryDTO registryDto);
    Task<APIRegistryDTO> CreateAPIRegistryAsync(CreateAPIRegistryDTO registryDto);
    Task DeleteAPIRegistryAsync(Guid id);
}