using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.TestRun;

public class TestJobDto
{
    public Guid JobId { get; set; }
    public Guid APIID { get; set; }
    public JobStatus Status { get; set; }
    public string? FailReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid? RatingId { get; set; }
}