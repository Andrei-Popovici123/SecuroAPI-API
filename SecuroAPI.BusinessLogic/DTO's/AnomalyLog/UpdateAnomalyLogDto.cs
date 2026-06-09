using System.ComponentModel.DataAnnotations;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;

public class UpdateAnomalyLogDto
{
    [Required]
    [StringLength(150)]
    public string AnomalyType { get; set; } = string.Empty;
    
    [Required]
    [EnumDataType(typeof(Severity))]
    public Severity Severity { get; set; }  

    [Required]
    public bool NotificationSent { get; set; }
}