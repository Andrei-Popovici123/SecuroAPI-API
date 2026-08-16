using SecuroAPI.DataAccess.Entities;

namespace SecuroAPI.DataAccess.Repositories.Interfaces;

public interface IScoreReportRepository: IRepository<ScoreReport>
{
    Task<IEnumerable<ScoreReport>> GetAllByRatingId(Guid APIID);
}