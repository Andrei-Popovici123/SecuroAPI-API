namespace SecuroAPI.Common.Constants;

public class MonitoringOptions
{
    public int TickSeconds { get; set; } = 60;
    public int BatchSize { get; set; } = 30;
    public int Concurrency { get; set; } = 5;
    public int ProbeTimeoutSeconds { get; set; } = 10;
}