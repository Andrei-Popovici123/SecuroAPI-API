namespace SecuroAPI.DataAccess.Entities;

public class MonitoredEndpoint
{
    public Guid EndpointId { get; set; }

    public Guid APIID { get; set; }
    public virtual APIRegistry ApiRegistry { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public int IntervalSeconds { get; set; } = 300;
    public DateTime? LastCheckedAt { get; set; }
    
    public bool? LastOk { get; set; }
    public int? LastStatusCode { get; set; }
    public int? LastLatencyMs { get; set; }
    public string? LastErrorType { get; set; }
    public bool HasHsts { get; set; }
    public bool HasCsp { get; set; }
    public bool HasNosniff { get; set; }
    public bool HasFrameOptions { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }

    public ICollection<TelemetryPoint> TelemetryPoints { get; set; } = new HashSet<TelemetryPoint>();
}