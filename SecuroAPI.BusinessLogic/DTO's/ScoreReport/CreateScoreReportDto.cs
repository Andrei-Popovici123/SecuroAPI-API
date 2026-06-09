using System.ComponentModel.DataAnnotations;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.ScoreReport;

public class CreateScoreReportDto
{
    [Required]
    [EnumDataType(typeof(Severity))]
    public Severity Severity { get; set; }

    [Required]
    [StringLength(500)] 
    public string Summary { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)] 
    public string Recommendation { get; set; } = string.Empty;

    [Required]
    public Guid RatingId { get; set; }

    [Required]
    public DateTime FinishedAt { get; set; }
}