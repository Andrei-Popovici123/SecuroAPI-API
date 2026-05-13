using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess.Entity;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class APIRegistryService : IAPIRegistryService
{
    private readonly IRepository<APIRegistry> _repository;

    public APIRegistryService(IRepository<APIRegistry> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<APIRegistryDTO>> GetAllAPIRegistriesAsync()
    {
        var registries = await _repository.GetAllAsync();
        return registries.Select(r => new APIRegistryDTO()
        {
            APIID = r.APIID,
            UserID = r.UserID,
            TargetURL = r.TargetURL,
            AuthType = r.AuthType,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt,
        });
    }

    public async Task<APIRegistryDTO?> GetAPIRegistryByIdAsync(Guid id)
    {
        var registry = await _repository.GetByIdAsync(id);
        if (registry == null)
        {
            return null;
        }

        return new APIRegistryDTO
        {
            APIID = registry.APIID,
            UserID = registry.UserID,
            TargetURL = registry.TargetURL,
            AuthType = registry.AuthType,
            Status = registry.Status,
            CreatedAt = registry.CreatedAt,
            LastModifiedAt = registry.LastModifiedAt,
        };
    }

    public async Task<APIRegistryDTO?> UpdateAPIRegistryAsync(Guid id, UpdateAPIRegistryDTO registryDto)
    {
        var registry = await _repository.GetByIdAsync(id);
        if (registry == null)
        {
            return null;
        }

        registry.UserID = registryDto.UserId;
        registry.TargetURL = registryDto.TargetURL;
        registry.AuthType = registryDto.AuthType;
        registry.Status = registryDto.Status;
        registry.LastModifiedAt = DateTime.Now;
        var newRegistry = await _repository.UpdateAsync(registry);
        
        return new APIRegistryDTO
        {
            APIID = registry.APIID,
            UserID = registry.UserID,
            TargetURL = registry.TargetURL,
            AuthType = registry.AuthType,
            Status = registry.Status,
            CreatedAt = registry.CreatedAt,
            LastModifiedAt = registry.LastModifiedAt,
        };
    }

    public async Task<APIRegistryDTO> CreateAPIRegistryAsync(CreateAPIRegistryDTO registryDto)
    {
        var registry = new APIRegistry
        {
            UserID = Guid.Empty,
            TargetURL = registryDto.TargetURL,
            AuthType = registryDto.AuthType,
            Status = registryDto.Status,
            CreatedAt = DateTime.Now,
            LastModifiedAt = DateTime.Now,
        };
        var createdRegistry = await _repository.AddAsync(registry);
        return new APIRegistryDTO()
        {
            APIID = createdRegistry.APIID,
            UserID = createdRegistry.UserID,
            TargetURL = createdRegistry.TargetURL,
            AuthType = createdRegistry.AuthType,
            Status = createdRegistry.Status,
            CreatedAt = createdRegistry.CreatedAt,
            LastModifiedAt = createdRegistry.LastModifiedAt,
        };
    }

    public async Task DeleteAPIRegistryAsync(Guid id)
    {
        var registry = await _repository.GetByIdAsync(id);
        if (registry == null)
        {
            throw new Exception($"Registry with the id {id} has not been found");
        }

        await _repository.DeleteAsync(id);
    }
}