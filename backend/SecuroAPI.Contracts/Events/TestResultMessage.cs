namespace SecuroAPI.Contracts.Events;

public record TestResultMessage(Guid APIID, int ExitCode, string Output);