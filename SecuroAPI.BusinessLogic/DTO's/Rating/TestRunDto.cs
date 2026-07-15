using SecuroAPI.BusinessLogic.DTO_s.ScoreReport;

namespace SecuroAPI.BusinessLogic.DTO_s.Rating;

public class TestRunDto
{
    public Guid RatingId { get; set; }
        
    public int VulnerabilityScore { get; set; }
        
    public int NumberOfTests { get; set; }
        
    public int OverallScore { get; set; }
    
    public ICollection<ScoreReportDto> Models { get; set; } = new HashSet<ScoreReportDto>();
    public Guid APIID { get; set; }
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime? LastModifiedAt { get; set; }
}