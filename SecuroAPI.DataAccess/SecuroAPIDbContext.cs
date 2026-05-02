using Microsoft.EntityFrameworkCore;

namespace SecuroAPI.DataAccess;

public class SecuroAPIDbContext: DbContext
{
    public SecuroAPIDbContext(DbContextOptions<SecuroAPIDbContext> options): base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}