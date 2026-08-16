using SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.Common.Results;

namespace SecuroAPI.BusinessLogic.Services.Interfaces;

public interface IRatingService
{
    Task<Result<IEnumerable<RatingDto>>> GetAllRatingAsync();
    Task<Result<RatingDto>> GetRatingByIdAsync(Guid id);
    Task<Result<RatingDto>> UpdateRatingAsync(Guid id, UpdateRatingDto? ratingDto);
    Task<Result<RatingDto>> CreateRatingAsync(CreateRatingDto? ratingDto);
    Task<Result> DeleteRatingAsync(Guid id);
    Task<Result<IEnumerable<RatingDto>>> GetAllRatingsByAPIID(Guid id);
}