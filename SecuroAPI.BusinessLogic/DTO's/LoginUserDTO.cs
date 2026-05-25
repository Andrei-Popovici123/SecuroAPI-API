using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s;

public class LoginUserDTO
{
    [Required,EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required,MinLength(8)]
    public string Password { get; set; } = string.Empty;
}