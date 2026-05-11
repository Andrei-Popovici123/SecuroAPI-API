using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entity;

namespace SecuroAPI.DataAccess.Configurations;

public class APIRegistryConfiguration : IEntityTypeConfiguration<APIRegistry>
{
    public void Configure(EntityTypeBuilder<APIRegistry> builder)
    {
        builder.ToTable("APIRegistry")
            .HasKey(r => r.APIID);

        builder.Property(r => r.UserID)
            .HasMaxLength(36);

        builder.Property(r => r.TargetURL)
            .HasMaxLength(2048);

        builder.HasIndex(r => r.APIID).IsUnique();

        builder.Property(r => r.AuthType).HasMaxLength(50);

        builder.Property(r => r.Status).HasDefaultValue("Inactive").HasMaxLength(20);

        builder.Property(r => r.CreatedAt).HasColumnType("datetime");
        
        builder.Property(r => r.LastModifiedAt).HasColumnType("datetime");
    }
}