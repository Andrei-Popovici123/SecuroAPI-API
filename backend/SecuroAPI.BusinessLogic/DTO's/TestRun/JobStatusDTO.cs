using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.TestRun;


public record JobStatusDto(Guid JobId, JobStatus Status , DateTime OccurredAt, string? FailReason);
