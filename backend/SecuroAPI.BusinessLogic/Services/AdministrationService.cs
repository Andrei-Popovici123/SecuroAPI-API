using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class AdministrationService : IAdministrationService
{
    private readonly IAPIRegistryRepository _apiRegistryRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdministrationService(IAPIRegistryRepository apiRegistryRepository, UserManager<ApplicationUser> userManager)
    {
        _apiRegistryRepository = apiRegistryRepository;
        _userManager = userManager;
    }

    public async Task<Result<IEnumerable<GetRegisteredUserDTO>>> GetAllUnapprovedUsers()
    {
        var users = await _userManager.Users
            .Where(u => u.Status == UserStatus.Pending)
            .ToListAsync();
        var mappedUsers = users.Select(r => new GetRegisteredUserDTO
        {
            Email = r.Email!,
            FirstName = r.FirstName,
            LastName = r.LastName,
            Id = r.Id,
            Status = r.Status.ToString()
        });

        return Result<IEnumerable<GetRegisteredUserDTO>>.Success(mappedUsers);
    }

    public async Task<Result<IEnumerable<GetRegisteredUserDTO>>> GetAllUsers()
    {
        var users = await _userManager.Users
            .ToListAsync();
        var mappedUsers = users.Select(r => new GetRegisteredUserDTO
        {
            Email = r.Email!,
            FirstName = r.FirstName,
            LastName = r.LastName,
            Id = r.Id,
            Status = r.Status.ToString()
        });

        return Result<IEnumerable<GetRegisteredUserDTO>>.Success(mappedUsers);
    }

    public async Task<Result<IEnumerable<APIRegistryDTO>>> GetAllUnapprovedAPIs()
    {
        var registries = await _apiRegistryRepository
            .GetAllAsync(r => r.Status == APIStatus.Pending);
        var mappedRegistries = registries.Select(r => new APIRegistryDTO()
        {
            APIID = r.APIID,
            UserID = r.UserID,
            TargetURL = r.TargetURL,
            AuthType = r.AuthType,
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt,
        });

        return Result<IEnumerable<APIRegistryDTO>>.Success(mappedRegistries);
    }

    public async Task<Result> ApproveUser(string id)
    {
    
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result.Failure();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"User with ID '{id}' does not exist"));
            }

            user.Status = UserStatus.Approved;
            user.LastModifiedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);
            return Result.Success();
    }

    public async Task<Result> RejectUser(string id)
    {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result.Failure();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"User with ID '{id}' does not exist"));
            }

            user.Status = UserStatus.Banned;
            user.LastModifiedAt = DateTime.UtcNow;

             await _userManager.UpdateAsync(user);
             await _userManager.UpdateSecurityStampAsync(user);

            return Result.Success();
    }

    public async Task<Result> ApproveAPI(Guid id)
    {
            var apiRegistry = await _apiRegistryRepository.GetByIdAsync(id);
            if (apiRegistry == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"API with ID '{id}' does not exist"));
            }
            if (apiRegistry.VerifiedAt is null)
                return Result.Failure(new Error(ErrorCodes.BadRequest, "Target ownership has not been verified."));
            if (apiRegistry.VerifiedAt is null)
                return Result.Failure(new Error(ErrorCodes.BadRequest,
                    "Target ownership has not been verified."));
            
            apiRegistry.Status = APIStatus.Approved;
            apiRegistry.LastModifiedAt = DateTime.UtcNow;

            await _apiRegistryRepository.UpdateAsync(apiRegistry);

            return Result.Success();
        
    }

    public async Task<Result> RejectAPI(Guid id)
    {

            var apiRegistry = await _apiRegistryRepository.GetByIdAsync(id);
            if (apiRegistry == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"API with ID '{id}' does not exist"));
            }

            apiRegistry.Status = APIStatus.Inactive;
            apiRegistry.LastModifiedAt = DateTime.UtcNow;

            await _apiRegistryRepository.UpdateAsync(apiRegistry);

            return Result.Success();

    }
}