using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IUserService
{
    Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto);
    Task<Result<string>> LoginUserAsync(LoginUserDTO loginUserDto);
    
}