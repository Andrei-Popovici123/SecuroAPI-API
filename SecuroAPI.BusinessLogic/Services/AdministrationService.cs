using SecuroAPI.BusinessLogic.DTO_s.APIRegistry;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services;

public class AdministrationService :IAdministrationService
{
    public Task<IEnumerable<Result<GetRegisteredUserDTO>>> GetAllUnapprovedUsers()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Result<GetRegisteredUserDTO>>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Result<APIRegistryDTO>>> GetAllUnapprovedAPIs()
    {
        throw new NotImplementedException();
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