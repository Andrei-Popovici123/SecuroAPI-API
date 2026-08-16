using Microsoft.AspNetCore.Identity;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.DataAccess.Entities;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyName { get; set; }= string.Empty;
    public UserStatus Status { get; set; } = UserStatus.New;
    
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public ICollection<APIRegistry> Registries { get; set; } = new HashSet<APIRegistry>();
}