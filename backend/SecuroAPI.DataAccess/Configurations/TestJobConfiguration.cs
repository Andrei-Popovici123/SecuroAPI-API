using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Configurations;

public class TestJobConfiguration : IEntityTypeConfiguration<TestJob>
{
    public void Configure(EntityTypeBuilder<TestJob> builder)
    {
        builder.HasKey(j => j.JobId);

   builder.HasOne(t => t.ApiRegistry)
            .WithMany(r=> r.TestJobs) 
            .HasForeignKey(x => x.APIID)
            .OnDelete(DeleteBehavior.Restrict);
   
   builder.Property(t => t.FailReason).HasMaxLength(500);
    }
}