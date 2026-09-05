using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.ScoreReport;

public class ScoreReportDto
{
    public Guid ReportId { get; set; }
        
    public Severity Severity { get; set; }

    public string Check { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public string Recommendation { get; set; } = string.Empty;
    
    public string? Evidence { get; set; }
    public Guid RatingId { get; set; }
        
    public DateTime FinishedAt { get; set; }
}