using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class APIRegistryService : IAPIRegistryService
{
    private readonly IAPIRegistryRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public APIRegistryService(IAPIRegistryRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
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
                .Failure(new Error(ErrorCodes.NotFound, $"API with the Id' {id} ' was not found"));

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

    public async Task<Result<IEnumerable<APIRegistryDTO>>> GetAllAPIRegistriesByUserID(string id)
    {
        var userId = _httpContextAccessor?
            .HttpContext?
            .User?.
            FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        
        
        var registries = await _repository.GetAllByUserId(id);
        var mappedRegistries = registries.Select(r => new APIRegistryDTO
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

    public async Task<Result<APIRegistryDTO>> UpdateAPIRegistryAsync(Guid id, UpdateAPIRegistryDTO? registryDto)
    {
        try
        {
            if (registryDto == null) return Result<APIRegistryDTO>.BadRequest();
            var registry = await _repository.GetByIdAsync(id);

            if (registry == null)
                return Result<APIRegistryDTO>
                    .Failure(new Error(ErrorCodes.NotFound, $"API with the Id' {id} ' was not found"));


            registry.UserID = registryDto.UserID;
            registry.TargetURL = registryDto.TargetURL.Trim();
            registry.AuthType = registryDto.AuthType;
            registry.Status = APIStatus.Pending;
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
            if (registryDto == null) return Result<APIRegistryDTO>.BadRequest();

            var exists = await APIRegistryExists(registryDto.TargetURL);
            if (exists)
            {
                return Result<APIRegistryDTO>.Failure(new Error("Conflict",
                    $"API with the URL' {registryDto.TargetURL} ' is already Registered"));
            }

            var registry = new APIRegistry
            {
                UserID = registryDto.UserID,
                TargetURL = registryDto.TargetURL.Trim(),
                AuthType = registryDto.AuthType,
                Status = APIStatus.Inactive,
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
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"The API with ID '{id}' does not exist"));

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