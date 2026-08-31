using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.DTO_s.TestRun;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.Services;

public class ScoringService : IScoringService
{
    private const int MaxScore = 100;
    private const int PenaltyCap = 99;
    
    private static readonly IReadOnlyDictionary<Severity, int> SeverityWeights =
        new Dictionary<Severity, int>
        {
            [Severity.Critical] = 40,
            [Severity.High] = 20,
            [Severity.Medium] = 8,
            [Severity.Low] = 2,
        };

    private static readonly IReadOnlyDictionary<Severity, double> DecayRates =
        new Dictionary<Severity, double>
        {
            [Severity.Critical] = 0.03,
            [Severity.High] = 0.02,
            [Severity.Medium] = 0.015,
            [Severity.Low] = 0.008,
        };
    
    public ScanScoreDto CalculateScore(IReadOnlyList<ScanFinding> reportFindings)
    {
        var scorePerSeverity = reportFindings
            .GroupBy(f => f.Severity)
            .ToDictionary(g => g.Key,
                g => g.Sum(_ => SeverityWeights.GetValueOrDefault(g.Key, 0)));

        // vulnerability score is the weighted sum of all vulnerabilities. Uncapped.
        var vulnerabilityScore = scorePerSeverity.Values.Sum();

        // the exponent is the sum of the scores per severity multiplied by their respective decay rates
        var exponet = scorePerSeverity
            .Sum(kv => DecayRates.GetValueOrDefault(kv.Key, 0) * kv.Value);

        var penalty = PenaltyCap * (1 - Math.Exp(-exponet));

        // Overall Score is capped max 100 the weight value of
        // severity decays with the increasing number of vulnerabilities
        var overallScore = (int)Math.Round(MaxScore - penalty);

        var scoreDto = new ScanScoreDto
        {
            OverallScore = overallScore,
            VulnerabilityScore = vulnerabilityScore
        };
        return scoreDto;
    }
}