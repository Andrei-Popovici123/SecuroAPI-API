using SecuroAPI.Common.Enums;

namespace SecuroAPI.Common.Models;

public record CatalogEntry(
    Guid          Id,       
    string        CheckId,    
    OwaspCategory Category,     
    string        Name,        
    string        Description,  
    Severity      Severity);   