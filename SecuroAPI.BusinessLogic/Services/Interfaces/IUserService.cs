using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IUserService
{
    Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto, string role,
        UserStatus status);

    Task<Result<string>> LoginUserAsync(LoginUserDTO loginUserDto);
}