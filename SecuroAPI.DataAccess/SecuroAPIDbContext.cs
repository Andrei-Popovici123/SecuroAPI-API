using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Configurations;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess;

public class SecuroAPIDbContext: IdentityDbContext<ApplicationUser>
{
    public SecuroAPIDbContext(DbContextOptions<SecuroAPIDbContext> options): base(options){}

    public DbSet<APIRegistry> APIRegistries { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        new APIRegistryConfiguration().Configure(modelBuilder.Entity<APIRegistry>());
    }
}