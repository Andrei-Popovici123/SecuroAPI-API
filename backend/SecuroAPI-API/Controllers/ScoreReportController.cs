using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.ScoreReport;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApprovedUser")]
    public class ScoreReportController : BaseFunctionalController
    {
        private readonly IScoreReportService _scoreReportService;

        public ScoreReportController(IScoreReportService scoreReportService)
        {
            _scoreReportService = scoreReportService;
        }

        /// <summary>
        /// Crud Operations for ScoreReport
        /// </summary>
        /// <returns></returns>
        ///
        [HttpGet]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<IEnumerable<ScoreReportDto>>> Get()
        {
            var scoreReports = await _scoreReportService.GetAllScoreReportAsync();
            return ToActionResult(scoreReports);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ScoreReportDto>> GetById(Guid id)
        {
            var scoreReport = await _scoreReportService.GetScoreReportByIdAsync(id);
            return ToActionResult(scoreReport);
        }


        [HttpGet("allByRatingId/{id:guid}")]
        public async Task<ActionResult<IEnumerable<ScoreReportDto>>> GetByRatingId(Guid id)
        {
            var scoreReport = await _scoreReportService.GetAllScoreReportsByRatingId(id);
            return ToActionResult(scoreReport);
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<ScoreReportDto>> Post([FromBody] CreateScoreReportDto scoreReportDto)
        {
            var newScoreReportResult = await _scoreReportService.CreateScoreReportAsync(scoreReportDto);
            
            if (!newScoreReportResult.IsSuccess) return MapErrorToResponse(newScoreReportResult.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = newScoreReportResult.Value!.ReportId },
                newScoreReportResult.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<ScoreReportDto>> Put(Guid id, [FromBody] UpdateScoreReportDto scoreReportDto)
        {
            var registry = await _scoreReportService.UpdateScoreReportAsync(id, scoreReportDto);
            return ToActionResult(registry);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedScoreReport = await _scoreReportService.DeleteScoreReportAsync(id);
            return ToActionResult(deletedScoreReport);
        }
    }
}