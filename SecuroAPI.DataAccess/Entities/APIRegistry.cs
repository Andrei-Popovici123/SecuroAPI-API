namespace SecuroAPI.DataAccess.Entity;

public class APIRegistry
{
    public Guid APIID { get; set; }
    public Guid UserID { get; set; }
    
    public string TargetURL { get; set; }
    public string AuthType { get; set; }

    public string Status { get; set; }
    //public virtual User User {get; set:} =null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
}