using SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IAnomalyLogService
{
    Task<Result<IEnumerable<AnomalyLogDto>>> GetAllAnomalyLogAsync();
    Task<Result<AnomalyLogDto>> GetAnomalyLogByIdAsync(Guid id);
    Task<Result<AnomalyLogDto>> UpdateAnomalyLogAsync(Guid id, UpdateAnomalyLogDto? anomalyLogDto);
    Task<Result<AnomalyLogDto>> CreateAnomalyLogAsync(CreateAnomalyLogDto? anomalyLogDto);
    Task<Result> DeleteAnomalyLogAsync(Guid id);
    Task<Result<IEnumerable<AnomalyLogDto>>> GetAllAnomalyLogByAPIID(Guid id);
    Task<Result<IEnumerable<AnomalyLogDto>>> GetMyAnomalyLogsAsync();
}