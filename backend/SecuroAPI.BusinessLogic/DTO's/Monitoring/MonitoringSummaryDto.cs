namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public class MonitoringSummaryDto
{
    public Guid APIID { get; set; }
    public int EndpointCount { get; set; }
    public int ActiveEndpointCount { get; set; }
    public int EndpointsUp { get; set; }
    public int EndpointsDown { get; set; }

    public double? UptimePercent { get; set; }
    public int? AvgLatencyMs { get; set; }
    public int? P95LatencyMs { get; set; }
    public int ProbeCount { get; set; }

    public int WindowHours { get; set; }
    public DateTime? LastCheckedAt { get; set; }

}