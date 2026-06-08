using SecuroAPI.Common.Enums;

namespace SecuroAPI.DataAccess.Entities;

public class AnomalyLog
{
    public Guid AnomalyId { get; set; }
    
    public string AnomalyType { get; set; } = string.Empty;
    
    public  Severity Severity{ get; set; }  
    
    public bool NotificationSent { get; set; }

    public Guid APIID { get; set; }
    
    public virtual APIRegistry ApiRegistry { get; set; } = null!;
}