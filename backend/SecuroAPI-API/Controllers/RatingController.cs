using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Rating;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingController : BaseFunctionalController
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        /// <summary>
        /// Crud Operations for Rating
        /// </summary>
        /// <returns></returns>
        ///
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatingDto>>> Get()
        {
            var ratings = await _ratingService.GetAllRatingAsync();
            return ToActionResult(ratings);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RatingDto>> GetById(Guid id)
        {
            var rating = await _ratingService.GetRatingByIdAsync(id);
            return ToActionResult(rating);
        }


        [HttpGet("APIID/{id:guid}")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetByAPIID(Guid id)
        {
            var rating = await _ratingService.GetAllRatingsByAPIID(id);
            return ToActionResult(rating);
        }

        [HttpPost]
        public async Task<ActionResult<RatingDto>> Post([FromBody] CreateRatingDto ratingDto)
        {
            var newRatingResult = await _ratingService.CreateRatingAsync(ratingDto);
            
            if (!newRatingResult.IsSuccess) return MapErrorToResponse(newRatingResult.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = newRatingResult.Value!.APIID },
                newRatingResult.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<RatingDto>> Put(Guid id, [FromBody] UpdateRatingDto ratingDto)
        {
            var registry = await _ratingService.UpdateRatingAsync(id, ratingDto);
            return ToActionResult(registry);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedRating = await _ratingService.DeleteRatingAsync(id);
            return ToActionResult(deletedRating);
        }
    }
}