namespace SecuroAPI.BusinessLogic.DTO_s.TestRun;

public record JobResultDto(Guid JobId, Guid APIID, int ExitCode, string Output);