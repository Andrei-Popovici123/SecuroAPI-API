using SecuroAPI.BusinessLogic.DTO_s;
using SecuroAPI.BusinessLogic.DTO_s.Auth;
using SecuroAPI.Common.Enums;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IUserService
{
    string UserId { get; }
    Task<Result<GetRegisteredUserDTO>> RegisterUserAsync(RegisterUserDTO registerUserDto, string role,
        UserStatus status);
    
    Task<Result<UserStatus>> GetStatusAsync(string userId);
    Task<Result<GetRegisteredUserDTO>> GetByIdAsync(string userId);
    Task<Result<IEnumerable<GetRegisteredUserDTO>>> GetAllUsersAsync();

    Task<Result<string>> LoginUserAsync(LoginUserDTO loginUserDto);
    Task<Result<GetRegisteredUserDTO>> UpdateProfileAsync(UpdateProfileDTO dto);
}