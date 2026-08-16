using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class AnomalyLogConfiguration : IEntityTypeConfiguration<AnomalyLog>
{
    public void Configure(EntityTypeBuilder<AnomalyLog> builder)
    {
        builder.ToTable("AnomalyLogs")
            .HasKey(l => l.AnomalyId);

        builder.Property(l => l.AnomalyType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(l => l.Severity)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.NotificationSent)
            .HasDefaultValue(false);
        
        builder.Property(l=>l.TimeStamp).HasColumnType("datetime");
        
        builder.HasOne(l => l.ApiRegistry)
            .WithMany(r => r.AnomalyLogs)
            .HasForeignKey(x => x.APIID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}