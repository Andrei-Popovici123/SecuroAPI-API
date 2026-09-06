using System.Reflection;
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
    public DbSet<ScoreReport> ScoreReports { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<AnomalyLog> AnomalyLogs { get; set; }
    public DbSet<TestConfig> TestConfigs { get; set; }
    public DbSet<TestJob> TestJobs { get; set; }
    
    public DbSet<MonitoredEndpoint> MonitoredEndpoints { get; set; }
    public DbSet<TelemetryPoint> TelemetryPoints { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}