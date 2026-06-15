using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
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
            .Where(u => u.Status==UserStatus.Pending)
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
        throw new NotImplementedException();
    }

    public async Task<Result> RejectUser(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> ApproveAPI(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RejectAPI(Guid id)
    {
        throw new NotImplementedException();
    }
}