using Microsoft.AspNetCore.Identity;

namespace SecuroAPI.DataAccess.Entities;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public bool Approved { get; set; }

    public ICollection<APIRegistry> Registries { get; set; } = new HashSet<APIRegistry>();
}