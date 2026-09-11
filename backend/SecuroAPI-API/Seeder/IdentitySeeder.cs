using Microsoft.AspNetCore.Identity;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Enums;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI_API.Seeder;

// SecuroAPI-API/Seeding/IdentitySeeder.cs
public static class IdentitySeeder
{
    public static async Task SeedAdminAsync(IServiceProvider sp)
    {
        var cfg   = sp.GetRequiredService<IConfiguration>();
        var users = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = sp.GetRequiredService<RoleManager<IdentityRole>>();

        var email    = cfg["SeedAdmin:Email"];
        var password = cfg["SeedAdmin:Password"];

        // no creds configured → no default admin (prod safety)
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        // idempotent: runs every startup, creates once
        if (await users.FindByEmailAsync(email) is not null) return;

        // belt-and-suspenders; your migration HasData normally creates this
        if (!await roles.RoleExistsAsync(RoleNames.Administrator))
            await roles.CreateAsync(new IdentityRole(RoleNames.Administrator));

        var admin = new ApplicationUser
        {
            UserName = email, Email = email, EmailConfirmed = true,
            FirstName = "System", LastName = "Administrator", CompanyName = "SecuroAPI",
            Status = UserStatus.Approved,
        };

        var result = await users.CreateAsync(admin, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                "Admin seed failed (password must meet the 12-char policy): " +
                string.Join("; ", result.Errors.Select(e => e.Description)));

        await users.AddToRoleAsync(admin, RoleNames.Administrator);
    }
}