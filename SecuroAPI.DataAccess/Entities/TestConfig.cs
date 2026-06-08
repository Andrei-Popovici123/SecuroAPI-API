namespace SecuroAPI.DataAccess.Entities;

public class TestConfig
{
    public Guid ConfigId { get; set; }
    public Guid Guid { get; set; }
    public List<Guid> EnabledTestIds { get; set; } = new();
}