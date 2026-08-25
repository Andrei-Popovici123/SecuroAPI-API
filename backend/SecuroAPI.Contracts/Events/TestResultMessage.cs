namespace SecuroAPI.Contracts.Events;

public record TestResultMessage(Guid APIID, string TargetUrl, string Output);