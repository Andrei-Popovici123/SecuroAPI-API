namespace SecuroAPI.BusinessLogic.DTO_s;

public class UpdateAPIRegistryDTO
{
    public Guid UserId { get; set; }
    public string TargetURL { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}