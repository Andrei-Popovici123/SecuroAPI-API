namespace SecuroAPI.BusinessLogic.DTO_s.TestConfig;

public class TestConfigDto
{
    public Guid ConfigId { get; set; }
        
    public Guid APIID { get; set; }
        
    public List<Guid> EnabledTestIds { get; set; } = new();
}