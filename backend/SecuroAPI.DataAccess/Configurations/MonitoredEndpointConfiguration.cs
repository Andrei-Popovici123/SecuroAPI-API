using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class MonitoredEndpointConfiguration : IEntityTypeConfiguration<MonitoredEndpoint>
{
    public void Configure(EntityTypeBuilder<MonitoredEndpoint> builder)
    {
        builder.HasKey(e => e.EndpointId);

        builder.Property(e => e.Url).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.Label).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastErrorType).HasMaxLength(100);

        builder.HasOne(e => e.ApiRegistry)
            .WithMany() // add a nav on APIRegistry if you want it
            .HasForeignKey(e => e.APIID)
            .OnDelete(DeleteBehavior.Cascade);

        // the scheduler's due-query
        builder.HasIndex(e => new { e.IsActive, e.LastCheckedAt });

        // one row per URL per target
        builder.HasIndex(e => new { e.APIID, e.Url }).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_MonitoredEndpoint_Interval", "[IntervalSeconds] >= 60"));
    }
}