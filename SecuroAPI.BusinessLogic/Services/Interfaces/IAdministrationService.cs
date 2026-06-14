using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IAdministrationService
{
    public Task<IEnumerable<Result<GetRegisteredUserDTO>>> GetAllUnapprovedUsers();
    public Task<IEnumerable<Result<GetRegisteredUserDTO>>> GetAllUsers();    
    public Task<IEnumerable<Result<APIRegistryDTO>>> GetAllUnapprovedAPIs();
    
    public Task<Result> ApproveUser(string id);
    public Task<Result> RejectUser(string id);
    public Task<Result> ApproveAPI(Guid id);
    public Task<Result> RejectAPI(Guid id);
}