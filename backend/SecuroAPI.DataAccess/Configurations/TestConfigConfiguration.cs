using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class TestConfigConfiguration : IEntityTypeConfiguration<TestConfig>
{
    public void Configure(EntityTypeBuilder<TestConfig> builder)
    {
        builder.ToTable("TestConfigs")
            .HasKey(x => x.ConfigId);

        builder.HasOne(t => t.ApiRegistry)
            .WithOne(r => r.TestConfig)
            .HasForeignKey<TestConfig>(t => t.APIID)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.EnabledTestIds);
    }
}