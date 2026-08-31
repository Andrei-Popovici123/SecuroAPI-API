using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IScoringService
{
    ScanScoreDto CalculateScore(IReadOnlyList<ScanFinding> findings);
}