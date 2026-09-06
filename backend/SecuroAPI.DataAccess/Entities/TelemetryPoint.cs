namespace SecuroAPI.DataAccess.Entities;

public class TelemetryPoint
{
    public long Id { get; set; }

    public Guid EndpointId { get; set; }
    public virtual MonitoredEndpoint Endpoint { get; set; } = null!;

    public DateTime CheckedAt { get; set; }
    public bool Ok { get; set; }
    public int? StatusCode { get; set; }
    public int LatencyMs { get; set; }
    public string? ErrorType { get; set; }
}