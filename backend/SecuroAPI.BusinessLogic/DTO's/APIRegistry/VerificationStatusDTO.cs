namespace SecuroAPI.BusinessLogic.DTO_s.APIRegistry;

public class VerificationStatusDTO
{
    public Guid APIID { get; set; }
    public bool Verified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string RecordName { get; set; } = string.Empty;
    public string ExpectedValue { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}