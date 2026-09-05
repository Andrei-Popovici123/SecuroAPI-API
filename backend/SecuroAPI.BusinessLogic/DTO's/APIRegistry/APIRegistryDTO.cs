using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.APIRegistry;

public class APIRegistryDTO
{
    public Guid APIID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string TargetURL { get; set; } = string.Empty;
    public string AuthType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    
    public string VerificationToken { get; set; } = string.Empty;
    public DateTime? VerifiedAt { get; set; }
}