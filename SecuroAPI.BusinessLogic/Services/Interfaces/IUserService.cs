using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.BusinessLogic.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IUserService
{
    Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto, string role);
    Task<Result<string>> LoginUserAsync(LoginUserDTO loginUserDto);
    
}