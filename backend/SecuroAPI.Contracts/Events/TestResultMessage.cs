namespace SecuroAPI.Contracts.Events;

public record TestResultMessage(Guid JobId, Guid APIID, int ExitCode, string Output) : RunnerMessage(JobId);