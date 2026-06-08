namespace SecuroAPI.DataAccess.Entities;

public class TestConfig
{
    public Guid ConfigId { get; set; }
    public Guid APIID { get; set; }
    public virtual APIRegistry ApiRegistry { get; set; } = null!;
    public List<Guid> EnabledTestIds { get; set; } = new();
}