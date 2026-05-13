namespace SecuroAPI.BusinessLogic.DTO_s;

public class APIRegistryDTO
{
    public Guid APIID { get; set; }
    public Guid? UserID { get; set; }
    public string TargetURL { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}