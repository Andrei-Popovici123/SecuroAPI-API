using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecuroAPI.DataAccess.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "9780c339-65f4-42fd-80d9-027e072bd570",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "82d29c6e-0376-4709-80fe-7d23604055f3"
            },
            new IdentityRole
            {
                Id = "c1f36078-5058-43eb-909d-14dbfbe1182d",
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "48632ef6-990b-461a-914f-f992af2c9568"
            }
        );
    }
}