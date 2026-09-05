using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.Auth;

public class LoginUserDTO
{
    [Required,EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required,MaxLength(128)]
    public string Password { get; set; } = string.Empty;
}