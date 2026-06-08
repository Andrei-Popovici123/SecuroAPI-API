using SecuroAPI.Common.Enums;

namespace SecuroAPI.DataAccess.Entities;

public class ScoreReport
{
    public Guid ReportId { get; set; }
    
    public Severity Severity{ get; set; }

    public string Summary { get; set; } = string.Empty;

    public string Recommendation { get; set; } = String.Empty;
    
    public Guid RatingId { get; set; }
    
    public virtual Rating Rating { get; set; } = null!;
    
    public DateTime FinishedAt { get; set; }
}