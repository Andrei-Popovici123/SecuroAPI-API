using SecuroAPI.BusinessLogic.DTO_s.ScoreReport;
using SecuroAPI.Common.Enums;

namespace SecuroAPI.BusinessLogic.DTO_s.TestRun;

public record ScanReport(
    string Target,
    DateTime StartedAt,
    DateTime FinishedAt,
    IReadOnlyList<string> ChecksRun,
    IReadOnlyList<ScanFinding> Findings);

public record ScanFinding(
    string Check,
    Severity Severity,
    string Summary,
    string Recommendation,
    ScanEvidence? Evidence);

public record ScanEvidence(string Url, string Indicator);