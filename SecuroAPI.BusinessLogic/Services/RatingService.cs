using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;
using SecuroAPI.Common.Results;
using SecuroAPI.DataAccess.Entities;
using SecuroAPI.DataAccess.Repositories.Interfaces;

namespace SecuroAPI.BusinessLogic.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _repository;

    public RatingService(IRatingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<RatingDto>>> GetAllRatingAsync()
    {
        var rating = await _repository.GetAllAsync();
        var mappedRating = rating.Select(r => new RatingDto()
        {
            RatingId = r.RatingId,
            VulnerabilityScore = r.VulnerabilityScore,
            NumberOfTests = r.NumberOfTests,
            OverallScore = r.OverallScore,
            APIID = r.APIID,
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt
        });

        return Result<IEnumerable<RatingDto>>.Success(mappedRating);
    }

    public async Task<Result<RatingDto>> GetRatingByIdAsync(Guid id)
    {
        var rating = await _repository.GetByIdAsync(id);

        if (rating == null)
            return Result<RatingDto>
                .Failure(new Error(ErrorCodes.NotFound, $"Rating with the Id' {id} ' was not found"));

        return Result<RatingDto>.Success(new RatingDto
        {
            RatingId = rating.RatingId,
            VulnerabilityScore = rating.VulnerabilityScore,
            NumberOfTests = rating.NumberOfTests,
            OverallScore = rating.OverallScore,
            APIID = rating.APIID,
            CreatedAt = rating.CreatedAt,
            LastModifiedAt = rating.LastModifiedAt
        });
    }

    public async Task<Result<RatingDto>> UpdateRatingAsync(Guid id, UpdateRatingDto? ratingDto)
    {
        try
        {
            if (ratingDto == null) return Result<RatingDto>.BadRequest();
            var rating = await _repository.GetByIdAsync(id);

            if (rating == null)
                return Result<RatingDto>
                    .Failure(new Error(ErrorCodes.NotFound, $"Rating with the Id' {id} ' was not found"));

            rating.VulnerabilityScore = ratingDto.VulnerabilityScore;
            rating.NumberOfTests = ratingDto.NumberOfTests;
            rating.OverallScore = ratingDto.OverallScore;
            rating.LastModifiedAt = DateTime.UtcNow;

            var updatedRating = await _repository.UpdateAsync(rating);

            return Result<RatingDto>.Success(new RatingDto
            {
                RatingId = updatedRating.RatingId,
                VulnerabilityScore = updatedRating.VulnerabilityScore,
                NumberOfTests = updatedRating.NumberOfTests,
                OverallScore = updatedRating.OverallScore,
                APIID = updatedRating.APIID,
                CreatedAt = updatedRating.CreatedAt,
                LastModifiedAt = updatedRating.LastModifiedAt
            });
        }
        catch (Exception)
        {
            return Result<RatingDto>.Failure();
        }
    }

    public async Task<Result<RatingDto>> CreateRatingAsync(CreateRatingDto? ratingDto)
    {
        try
        {
            if (ratingDto == null) return Result<RatingDto>.BadRequest();

            var rating = new Rating
            {
                VulnerabilityScore = ratingDto.VulnerabilityScore,
                NumberOfTests = ratingDto.NumberOfTests,
                OverallScore = ratingDto.OverallScore,
                APIID = ratingDto.APIID,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            var newRating = await _repository.AddAsync(rating);


            return Result<RatingDto>.Success(new RatingDto
            {
                RatingId = newRating.RatingId,
                VulnerabilityScore = newRating.VulnerabilityScore,
                NumberOfTests = newRating.NumberOfTests,
                OverallScore = newRating.OverallScore,
                APIID = newRating.APIID,
                CreatedAt = newRating.CreatedAt,
                LastModifiedAt = newRating.LastModifiedAt
            });
        }
        catch (Exception)
        {
            return Result<RatingDto>.Failure();
        }
    }

    public async Task<Result> DeleteRatingAsync(Guid id)
    {
        try
        {
            var rating = await _repository.GetByIdAsync(id);
            if (rating == null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Rating with ID '{id}' does not exist"));

            await _repository.DeleteAsync(id);
            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    public async Task<Result<IEnumerable<RatingDto>>> GetAllRatingsByAPIID(Guid id)
    {
        var rating = await _repository.GetAllByAPIID(id);
        var mappedRating = rating.Select(r => new RatingDto()
        {
            RatingId = r.RatingId,
            VulnerabilityScore = r.VulnerabilityScore,
            NumberOfTests = r.NumberOfTests,
            OverallScore = r.OverallScore,
            APIID = r.APIID,
            CreatedAt = r.CreatedAt,
            LastModifiedAt = r.LastModifiedAt
        });

        return Result<IEnumerable<RatingDto>>.Success(mappedRating);
    }
}