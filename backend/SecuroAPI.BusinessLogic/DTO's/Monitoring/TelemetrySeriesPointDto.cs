namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public class TelemetrySeriesPointDto
{
    public DateTime BucketStart { get; set; }
    public int AvgLatencyMs { get; set; }
    public int MaxLatencyMs { get; set; }
    public int ProbeCount { get; set; }
    public int FailureCount { get; set; }
}