using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.AnomalyLog;
using SecuroAPI.BusinessLogic.Services.Interfaces;
using SecuroAPI.Common.Constants;

namespace SecuroAPI_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "ApprovedUser")]
    public class AnomalyLogController : BaseFunctionalController
    {
        private readonly IAnomalyLogService _anomalyLogService;

        public AnomalyLogController(IAnomalyLogService anomalyLogService)
        {
            _anomalyLogService = anomalyLogService;
        }

        /// <summary>
        /// Crud Operations for AnomalyLog
        /// </summary>
        /// <returns></returns>
        ///
        [Authorize(Roles = RoleNames.Administrator)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnomalyLogDto>>> Get()
        {
            var anomalyLogs = await _anomalyLogService.GetAllAnomalyLogAsync();
            return ToActionResult(anomalyLogs);
        }
        
        [HttpGet("myLogs")]
        public async Task<ActionResult<IEnumerable<AnomalyLogDto>>> GetMine()
            => ToActionResult(await _anomalyLogService.GetMyAnomalyLogsAsync());

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AnomalyLogDto>> GetById(Guid id)
        {
            var anomalyLog = await _anomalyLogService.GetAnomalyLogByIdAsync(id);
            return ToActionResult(anomalyLog);
        }


        [HttpGet("allByApiId/{id:guid}")]
        public async Task<ActionResult<IEnumerable<AnomalyLogDto>>> GetByAPIID(Guid id)
        {
            var anomalyLog = await _anomalyLogService.GetAllAnomalyLogByAPIID(id);
            return ToActionResult(anomalyLog);
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<AnomalyLogDto>> Post([FromBody] CreateAnomalyLogDto anomalyLogDto)
        {
            var newAnomalyLogResult = await _anomalyLogService.CreateAnomalyLogAsync(anomalyLogDto);
            
            if (!newAnomalyLogResult.IsSuccess) return MapErrorToResponse(newAnomalyLogResult.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = newAnomalyLogResult.Value!.APIID },
                newAnomalyLogResult.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<ActionResult<AnomalyLogDto>> Put(Guid id, [FromBody] UpdateAnomalyLogDto anomalyLogDto)
        {
            var registry = await _anomalyLogService.UpdateAnomalyLogAsync(id, anomalyLogDto);
            return ToActionResult(registry);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = RoleNames.Administrator)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedAnomalyLog = await _anomalyLogService.DeleteAnomalyLogAsync(id);
            return ToActionResult(deletedAnomalyLog);
        }
    }
}