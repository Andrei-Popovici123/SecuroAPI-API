using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings")
            .HasKey(x => x.RatingId);

        builder.Property(x => x.VulnerabilityScore)
            .IsRequired();

        builder.Property(x => x.NumberOfTests)
            .IsRequired();

        builder.Property(x => x.OverallScore)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.Registry)
            .WithMany(r=>r.Ratings)
            .HasForeignKey(x => x.RegistryId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}