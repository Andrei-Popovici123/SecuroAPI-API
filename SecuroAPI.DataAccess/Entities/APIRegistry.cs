using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.DataAccess.Entities;

public class APIRegistry
{
    public Guid APIID { get; set; }
    public Guid UserID { get; set; } 
    
    [Url]
    public string TargetURL { get; set; }= string.Empty;
    
    public string AuthType { get; set; }= string.Empty;

    public string Status { get; set; } = "Inactive";
    //public virtual User User {get; set:} =null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}