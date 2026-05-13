namespace SecuroAPI.BusinessLogic.DTO_s;

public class CreateAPIRegistryDTO
{
    public Guid UserID { get; set; }
    public string TargetURL { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}