using System.ComponentModel.DataAnnotations;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.ScoreReport;

public class UpdateScoreReportDto
{
    [Required]
    [EnumDataType(typeof(Severity))]
    public Severity Severity { get; set; }

    [Required]
    [StringLength(1000)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Recommendation { get; set; } = string.Empty;
}