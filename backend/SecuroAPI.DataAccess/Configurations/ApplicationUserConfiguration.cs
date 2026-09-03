using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .HasMaxLength(100);
        
        builder.Property(u => u.LastName)
            .HasMaxLength(100);
        builder.Property(u => u.CompanyName)
            .HasMaxLength(200);
        
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(r => r.CreatedAt).HasColumnType("datetime2");

        builder.Property(r => r.LastModifiedAt).HasColumnType("datetime2");
        
        builder.Property(r => r.LastLoginAt).HasColumnType("datetime2");
    }
}