namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public class TelemetrySeriesDto
{
    public Guid EndpointId { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int WindowHours { get; set; }
    public List<TelemetrySeriesPointDto> Points { get; set; } = new();
}