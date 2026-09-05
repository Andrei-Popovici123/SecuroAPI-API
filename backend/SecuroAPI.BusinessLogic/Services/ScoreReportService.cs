using SecuroAPI.BusinessLogic.DTO_s.ScoreReport;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class ScoreReportService : IScoreReportService
{
    private readonly IScoreReportRepository _repository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IUserService _userService;

    public ScoreReportService(IScoreReportRepository repository, IRatingRepository ratingRepository,
        IUserService userService)
    {
        _repository = repository;
        _ratingRepository = ratingRepository;
        _userService = userService;
    }

    public async Task<Result<IEnumerable<ScoreReportDto>>> GetAllScoreReportAsync()
    {
        var scoreReport = await _repository.GetAllAsync();
        var mappedScoreReport = scoreReport.Select(MapToDto);

        return Result<IEnumerable<ScoreReportDto>>.Success(mappedScoreReport);
    }

    public async Task<Result<ScoreReportDto>> GetScoreReportByIdAsync(Guid id)
    {
        var scoreReport = await _repository.GetByIdAsync(id);

        if (scoreReport == null || !await OwnsRating(scoreReport.RatingId))
            return Result<ScoreReportDto>
                .Failure(new Error(ErrorCodes.NotFound, $"ScoreReport with the Id' {id} ' was not found"));

        return Result<ScoreReportDto>.Success(MapToDto(scoreReport));
    }

    public async Task<Result<ScoreReportDto>> UpdateScoreReportAsync(Guid id, UpdateScoreReportDto? scoreReportDto)
    {
            if (scoreReportDto == null) return Result<ScoreReportDto>.BadRequest();
            var scoreReport = await _repository.GetByIdAsync(id);

            if (scoreReport == null)
                return Result<ScoreReportDto>
                    .Failure(new Error(ErrorCodes.NotFound, $"ScoreReport with the Id' {id} ' was not found"));

            scoreReport.Severity = scoreReportDto.Severity;
            scoreReport.Summary = scoreReportDto.Summary;
            scoreReport.Recommendation = scoreReportDto.Recommendation;
            scoreReport.Check = scoreReportDto.Check;
            scoreReport.Evidence = scoreReportDto.Evidence;

            var updatedScoreReport = await _repository.UpdateAsync(scoreReport);

            return Result<ScoreReportDto>.Success(MapToDto(updatedScoreReport));
        
    }

    public async Task<Result<ScoreReportDto>> CreateScoreReportAsync(CreateScoreReportDto? scoreReportDto)
    {
            if (scoreReportDto == null) return Result<ScoreReportDto>.BadRequest();

            var scoreReport = new ScoreReport
            {
                Severity = scoreReportDto.Severity,
                Summary = scoreReportDto.Summary,
                Recommendation = scoreReportDto.Recommendation,
                RatingId = scoreReportDto.RatingId,
                FinishedAt = scoreReportDto.FinishedAt,
                Check = scoreReportDto.Check,
                Evidence = scoreReportDto.Evidence
            };

            var newScoreReport = await _repository.AddAsync(scoreReport);


            return Result<ScoreReportDto>.Success(MapToDto(newScoreReport));
    }

    public async Task<Result> DeleteScoreReportAsync(Guid id)
    {
        var scoreReport = await _repository.GetByIdAsync(id);
        if (scoreReport == null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"ScoreReport with ID '{id}' does not exist"));

        await _repository.DeleteAsync(id);
        return Result.Success();
    }


    public async Task<Result<IEnumerable<ScoreReportDto>>> GetAllScoreReportsByRatingId(Guid id)
    {
        if (!await OwnsRating(id))
            return Result<IEnumerable<ScoreReportDto>>.Failure(
                new Error(ErrorCodes.NotFound, $"Rating with the Id '{id}' was not found"));

        var scoreReport = await _repository.GetAllByRatingId(id);
        return Result<IEnumerable<ScoreReportDto>>.Success(
            scoreReport.OrderByDescending(sr => sr.Severity)
                .ThenBy(sr => sr.Check)
                .Select(MapToDto));
    }

    private async Task<bool> OwnsRating(Guid ratingId)
        => await _ratingRepository.CheckExistsAsync(r =>
            r.RatingId == ratingId && r.Registry.UserID == _userService.UserId);

    private static ScoreReportDto MapToDto(ScoreReport sr) => new()
    {
        ReportId = sr.ReportId,
        Severity = sr.Severity,
        Summary = sr.Summary,
        Recommendation = sr.Recommendation,
        RatingId = sr.RatingId,
        FinishedAt = sr.FinishedAt,
        Check = sr.Check,
        Evidence = sr.Evidence
    };
}