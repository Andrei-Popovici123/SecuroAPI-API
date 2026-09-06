namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public class TelemetrySeriesPointDto
{
    public DateTime CheckedAt { get; set; }
    public int LatencyMs { get; set; }
    public bool Ok { get; set; }
    public int? StatusCode { get; set; }
}