using System.ComponentModel.DataAnnotations;

namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public class CreateMonitoredEndpointDto
{
    [Required, Url, MaxLength(2048)]
    public string Url { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Label { get; set; } = string.Empty;

    [Range(60, 3600)]
    public int IntervalSeconds { get; set; } = 300;
}