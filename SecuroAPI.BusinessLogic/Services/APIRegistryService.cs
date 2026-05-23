using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.Results;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class APIRegistryService : IAPIRegistryService
{
    private readonly IRepository<APIRegistry> _repository;

    public APIRegistryService(IRepository<APIRegistry> repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<APIRegistryDTO>>> GetAllAPIRegistriesAsync()
    {
        var registries = await _repository.GetAllAsync();
        var mappedRegistries = registries.Select(r => new APIRegistryDTO()
        {
            APIID = r.APIID,
            UserID = r.UserID,
            TargetURL = r.TargetURL,
            AuthType = r.AuthType,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt,
        });

        return Result<IEnumerable<APIRegistryDTO>>.Success(mappedRegistries);
    }

    public async Task<Result<APIRegistryDTO>> GetAPIRegistryByIdAsync(Guid id)
    {
        var registry = await _repository.GetByIdAsync(id);

        if (registry == null)
            return Result<APIRegistryDTO>
                .Failure(new Error("NotFound", $"API with the Id' {id} ' was not found"));

        return Result<APIRegistryDTO>.Success(new APIRegistryDTO
        {
            APIID = registry.APIID,
            UserID = registry.UserID,
            TargetURL = registry.TargetURL,
            AuthType = registry.AuthType,
            Status = registry.Status,
            CreatedAt = registry.CreatedAt,
            LastModifiedAt = registry.LastModifiedAt,
        });
    }

    public async Task<Result<APIRegistryDTO>> UpdateAPIRegistryAsync(Guid id, UpdateAPIRegistryDTO? registryDto)
    {
        try
        {
            if (registryDto == null) return Result<APIRegistryDTO>.BadRequest();
            var registry = await _repository.GetByIdAsync(id);
            
            if (registry == null) return Result<APIRegistryDTO>
                .Failure(new Error("NotFound", $"API with the Id' {id} ' was not found"));
            
            var duplicateUrl = await APIRegistryExists(registryDto.TargetURL);
            if (duplicateUrl)
            {
                return Result<APIRegistryDTO>.Failure(new Error("Conflict",
                    $"API with the URL' {registryDto.TargetURL} ' is already Registered"));
            }


            registry.UserID = registryDto.UserId;
            registry.TargetURL = registryDto.TargetURL.Trim();
            registry.AuthType = registryDto.AuthType;
            registry.Status = registryDto.Status;
            registry.LastModifiedAt = DateTime.UtcNow;
            var newRegistry = await _repository.UpdateAsync(registry);

            return Result<APIRegistryDTO>.Success(new APIRegistryDTO
            {
                APIID = newRegistry.APIID,
                UserID = newRegistry.UserID,
                TargetURL = newRegistry.TargetURL,
                AuthType = newRegistry.AuthType,
                Status = newRegistry.Status,
                CreatedAt = newRegistry.CreatedAt,
                LastModifiedAt = newRegistry.LastModifiedAt,
            });
        }
        catch (Exception)
        {
            return Result<APIRegistryDTO>.Failure();
        }
    }

    public async Task<Result<APIRegistryDTO>> CreateAPIRegistryAsync(CreateAPIRegistryDTO? registryDto)
    {
        try
        {
            if (registryDto == null) return Result<APIRegistryDTO>.BadRequest();;

            var exists = await APIRegistryExists(registryDto.TargetURL);
            if (exists)
            {
                return Result<APIRegistryDTO>.Failure(new Error("Conflict",
                    $"API with the URL' {registryDto.TargetURL} ' is already Registered"));
            }

            var registry = new APIRegistry
            {
                UserID = Guid.Empty,
                TargetURL = registryDto.TargetURL.Trim(),
                AuthType = registryDto.AuthType,
                Status = registryDto.Status,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };

            var createdRegistry = await _repository.AddAsync(registry);

            return Result<APIRegistryDTO>.Success(new APIRegistryDTO
            {
                APIID = createdRegistry.APIID,
                UserID = createdRegistry.UserID,
                TargetURL = createdRegistry.TargetURL,
                AuthType = createdRegistry.AuthType,
                Status = createdRegistry.Status,
                CreatedAt = createdRegistry.CreatedAt,
                LastModifiedAt = createdRegistry.LastModifiedAt,
            });
        }
        catch (Exception)
        {
            return Result<APIRegistryDTO>.Failure();
        }
    }

    public async Task<Result> DeleteAPIRegistryAsync(Guid id)
    {
        try
        {
            var registry = await _repository.GetByIdAsync(id);
            if (registry == null)
                return Result.NotFound(new Error("NotFound", $"The API with ID '{id}' does not exist"));

            await _repository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    public async Task<bool> APIRegistryExists(string targetUrl)
    {
        if (string.IsNullOrWhiteSpace(targetUrl)) return false;
        string cleanedUrl = targetUrl.Trim();
        
        return await _repository
            .CheckExistsAsync(u => u.TargetURL == cleanedUrl);
    }
}