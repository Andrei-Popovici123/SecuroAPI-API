namespace SecuroAPI.BusinessLogic.DTO_s.Monitoring;

public record ProbeResult(
    bool Ok,
    int? StatusCode,
    int LatencyMs,
    string? ErrorType,
    bool Hsts,
    bool Csp,
    bool Nosniff,
    bool FrameOptions);