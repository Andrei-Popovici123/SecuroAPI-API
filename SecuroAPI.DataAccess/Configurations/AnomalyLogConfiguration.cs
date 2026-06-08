using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class AnomalyLogConfiguration : IEntityTypeConfiguration<AnomalyLog>
{
    public void Configure(EntityTypeBuilder<AnomalyLog> builder)
    {
        builder.ToTable("AnomalyLogs")
            .HasKey(x => x.AnomalyId);

        builder.Property(x => x.AnomalyType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Severity)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NotificationSent)
            .HasDefaultValue(false);
        
        builder.HasOne(l => l.ApiRegistry)
            .WithMany(r => r.AnomalyLogs)
            .HasForeignKey(x => x.APIID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}