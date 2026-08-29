using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.Common.Enums;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class ScoreReportConfiguration : IEntityTypeConfiguration<ScoreReport>
{
    public void Configure(EntityTypeBuilder<ScoreReport> builder)
    {
        builder.ToTable("ScoreReport")
            .HasKey(x => x.ReportId);


        builder.Property(x => x.Severity)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Summary)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Recommendation)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(s => s.Check)
            .HasMaxLength(100).IsRequired();
        
        builder.Property(s => s.Evidence)
            .HasMaxLength(1000);
        
        builder.Property(x => x.FinishedAt)
            .IsRequired();

        builder.HasOne(x => x.Rating)
            .WithMany(r => r.ScoreReports)
            .HasForeignKey(x => x.RatingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}