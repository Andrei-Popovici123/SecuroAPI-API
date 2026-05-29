namespace SecuroAPI.BusinessLogic.DTO_s.APIRegistry;

public class CreateAPIRegistryDTO
{
    public string UserID { get; set; } = string.Empty;
    public string TargetURL { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
    public string Status { get; set; } = "Inactive";
}