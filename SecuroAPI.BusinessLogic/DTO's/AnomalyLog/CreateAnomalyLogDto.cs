using System.ComponentModel.DataAnnotations;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;

public class CreateAnomalyLogDto
{
    [Required]
    [StringLength(150)]
    public string AnomalyType { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(Severity))]
    public Severity Severity { get; set; }

    public bool NotificationSent { get; set; }

    [Required]
    public Guid APIID { get; set; }
}