using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.Auth;

public class GetRegisteredUserDTO
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}