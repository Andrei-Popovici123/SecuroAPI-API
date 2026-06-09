using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;

public class AnomalyLogDto
{
    public Guid AnomalyId { get; set; }
        
    public string AnomalyType { get; set; } = string.Empty;
        
    public Severity Severity { get; set; }  
        
    public bool NotificationSent { get; set; }

    public Guid APIID { get; set; }
}