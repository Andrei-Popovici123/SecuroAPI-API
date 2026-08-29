using SecuroAPI.Common.Enums;

namespace SecuroAPI.DataAccess.Entities;

public class TestJob
{
    public Guid JobId { get; set; }
    public Guid APIID { get; set; }
    public JobStatus Status { get; set; }

    public string? FailReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    
    public Guid? RatingId { get; set; }

    public virtual APIRegistry ApiRegistry { get; set; } = null!;
    
}