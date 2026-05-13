using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.DataAccess.Entity;

public class APIRegistry
{
    public Guid APIID { get; set; }
    public Guid UserID { get; set; } 
    
    [Url]
    public string TargetURL { get; set; }= string.Empty;
    
    public string AuthType { get; set; }= string.Empty;

    public string Status { get; set; }= string.Empty;
    //public virtual User User {get; set:} =null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
}