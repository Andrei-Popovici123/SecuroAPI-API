namespace SecuroAPI.BusinessLogic.DTO_s.TestRun;

public class TestRunTriggeredDto()
{
    public Guid JobId { get; set; }
    public Guid APIID { get; set; }
    public string JobStatus { get; set; } = string.Empty;
}