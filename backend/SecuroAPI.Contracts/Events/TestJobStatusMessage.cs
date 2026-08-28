using SecuroAPI.Common.Enums;

namespace SecuroAPI.Contracts.Events;

public record TestJobStatusMessage(Guid JobId, JobStatus Status, DateTime OccurredAt, string? Error = null)
    : RunnerMessage(JobId);