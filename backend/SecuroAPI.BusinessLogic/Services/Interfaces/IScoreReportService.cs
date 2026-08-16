using SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;
using SecuroAPI.BusinessLogic.DTO_s.ScoreReport;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IScoreReportService
{
    Task<Result<IEnumerable<ScoreReportDto>>> GetAllScoreReportAsync();
    Task<Result<ScoreReportDto>> GetScoreReportByIdAsync(Guid id);
    Task<Result<ScoreReportDto>> UpdateScoreReportAsync(Guid id, UpdateScoreReportDto? scoreReportDto);
    Task<Result<ScoreReportDto>> CreateScoreReportAsync(CreateScoreReportDto? scoreReportDto);
    Task<Result> DeleteScoreReportAsync(Guid id);
    Task<Result<IEnumerable<ScoreReportDto>>> GetAllScoreReportsByRatingId(Guid id);
}