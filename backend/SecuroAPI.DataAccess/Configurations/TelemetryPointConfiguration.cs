using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class TelemetryPointConfiguration: IEntityTypeConfiguration<TelemetryPoint>
{
    public void Configure(EntityTypeBuilder<TelemetryPoint> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.ErrorType).HasMaxLength(100);

        builder.HasOne(t => t.Endpoint)
            .WithMany(e => e.TelemetryPoints)
            .HasForeignKey(t => t.EndpointId)
            .OnDelete(DeleteBehavior.Cascade);

        // dashboard series query, covering
        builder.HasIndex(t => new { t.EndpointId, t.CheckedAt })
            .IncludeProperties(t => new { t.Ok, t.LatencyMs, t.StatusCode });
    }
}