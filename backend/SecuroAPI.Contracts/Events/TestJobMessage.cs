namespace SecuroAPI.Contracts.Events;

public record TestJobMessage(
    Guid JobId,
    Guid APIID,
    string TargetUrl,
    IReadOnlyList<Guid> EnabledTestIds);