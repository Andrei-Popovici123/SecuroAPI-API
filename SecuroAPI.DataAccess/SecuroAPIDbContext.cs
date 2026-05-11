using Microsoft.EntityFrameworkCore;
using SecuroAPI.DataAccess.Configurations;
using SecuroAPI.DataAccess.Entity;

namespace SecuroAPI.DataAccess;

public class SecuroAPIDbContext: DbContext
{
    public SecuroAPIDbContext(DbContextOptions<SecuroAPIDbContext> options): base(options){}

    public DbSet<APIRegistry> APIRegistries { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        new APIRegistryConfiguration().Configure(modelBuilder.Entity<APIRegistry>());
    }
}