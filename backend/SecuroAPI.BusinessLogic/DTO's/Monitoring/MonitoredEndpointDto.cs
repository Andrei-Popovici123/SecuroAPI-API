namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public class MonitoredEndpointDto
{
    public Guid EndpointId { get; set; }
    public Guid APIID { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int IntervalSeconds { get; set; }

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
}