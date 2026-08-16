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

    public ScoreReportService(IScoreReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<ScoreReportDto>>> GetAllScoreReportAsync()
    {
        var scoreReport = await _repository.GetAllAsync();
        var mappedScoreReport = scoreReport.Select(sr => new ScoreReportDto()
        {
           ReportId= sr.ReportId,
           Severity= sr.Severity,
            Summary = sr.Summary,
            Recommendation = sr.Recommendation,
            RatingId = sr.RatingId,
            FinishedAt = sr.FinishedAt,
        });

        return Result<IEnumerable<ScoreReportDto>>.Success(mappedScoreReport);
    }

    public async Task<Result<ScoreReportDto>> GetScoreReportByIdAsync(Guid id)
    {
        var scoreReport = await _repository.GetByIdAsync(id);

        if (scoreReport == null)
            return Result<ScoreReportDto>
                .Failure(new Error(ErrorCodes.NotFound, $"ScoreReport with the Id' {id} ' was not found"));

        return Result<ScoreReportDto>.Success(new ScoreReportDto
        {
            ReportId= scoreReport.ReportId,
            Severity= scoreReport.Severity,
            Summary = scoreReport.Summary,
            Recommendation = scoreReport.Recommendation,
            RatingId = scoreReport.RatingId,
            FinishedAt = scoreReport.FinishedAt,
        });
    }

    public async Task<Result<ScoreReportDto>> UpdateScoreReportAsync(Guid id, UpdateScoreReportDto? scoreReportDto)
    {
        try
        {
            if (scoreReportDto == null) return Result<ScoreReportDto>.BadRequest();
            var scoreReport = await _repository.GetByIdAsync(id);

            if (scoreReport == null)
                return Result<ScoreReportDto>
                    .Failure(new Error(ErrorCodes.NotFound, $"ScoreReport with the Id' {id} ' was not found"));

            scoreReport.Severity = scoreReportDto.Severity;
            scoreReport.Summary = scoreReportDto.Summary;
            scoreReport.Recommendation = scoreReportDto.Recommendation;

            var updatedScoreReport = await _repository.UpdateAsync(scoreReport);

            return Result<ScoreReportDto>.Success(new ScoreReportDto
            {
                ReportId= updatedScoreReport.ReportId,
                Severity= updatedScoreReport.Severity,
                Summary = updatedScoreReport.Summary,
                Recommendation = updatedScoreReport.Recommendation,
                RatingId = updatedScoreReport.RatingId,
                FinishedAt = updatedScoreReport.FinishedAt,
            });
        }
        catch (Exception)
        {
            return Result<ScoreReportDto>.Failure();
        }
    }

    public async Task<Result<ScoreReportDto>> CreateScoreReportAsync(CreateScoreReportDto? scoreReportDto)
    {
        try
        {
            if (scoreReportDto == null) return Result<ScoreReportDto>.BadRequest();

            var scoreReport = new ScoreReport
            {
                Severity= scoreReportDto.Severity,
                Summary = scoreReportDto.Summary,
                Recommendation = scoreReportDto.Recommendation,
                RatingId = scoreReportDto.RatingId,
                FinishedAt = DateTime.UtcNow,
            };

            var newScoreReport = await _repository.AddAsync(scoreReport);


            return Result<ScoreReportDto>.Success(new ScoreReportDto
            {
                ReportId= newScoreReport.ReportId,
                Severity= newScoreReport.Severity,
                Summary = newScoreReport.Summary,
                Recommendation = newScoreReport.Recommendation,
                RatingId = newScoreReport.RatingId,
                FinishedAt = newScoreReport.FinishedAt,
            });
        }
        catch (Exception)
        {
            return Result<ScoreReportDto>.Failure();
        }
    }

    public async Task<Result> DeleteScoreReportAsync(Guid id)
    {
        try
        {
            var scoreReport = await _repository.GetByIdAsync(id);
            if (scoreReport == null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"ScoreReport with ID '{id}' does not exist"));

            await _repository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }
    

    public async Task<Result<IEnumerable<ScoreReportDto>>> GetAllScoreReportsByRatingId(Guid id)
    {
        var scoreReport = await _repository.GetAllByRatingId(id);
        var mappedScoreReport = scoreReport.Select(sr => new ScoreReportDto()
        {
            ReportId= sr.ReportId,
            Severity= sr.Severity,
            Summary = sr.Summary,
            Recommendation = sr.Recommendation,
            RatingId = sr.RatingId,
            FinishedAt = sr.FinishedAt,
        });

        return Result<IEnumerable<ScoreReportDto>>.Success(mappedScoreReport);
    }
}