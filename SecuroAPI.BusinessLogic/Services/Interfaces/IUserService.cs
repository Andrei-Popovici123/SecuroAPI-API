using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IUserService
{
    Task<Result<RegisterUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto);
    
}