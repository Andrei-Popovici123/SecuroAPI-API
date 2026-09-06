using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ApprovedUser")]
public class MonitoringDashboardController : BaseFunctionalController
{
    private readonly IMonitoringDashboardService _service;
    private readonly IMonitoredEndpointService _endpointService;

    public MonitoringDashboardController(
        IMonitoringDashboardService service,
        IMonitoredEndpointService endpointService)
    {
        _service = service;
        _endpointService = endpointService;
    }

    [HttpGet("api/{apiId:guid}/summary")]
    public async Task<ActionResult<MonitoringSummaryDto>> GetSummary(
        Guid apiId, [FromQuery] int windowHours = 24)
        => ToActionResult(await _service.GetSummaryAsync(apiId, windowHours));

    [HttpGet("api/{apiId:guid}/endpoints")]
    public async Task<ActionResult<IEnumerable<MonitoredEndpointDto>>> GetEndpoints(Guid apiId)
        => ToActionResult(await _endpointService.GetAllByAPIID(apiId));

    [HttpGet("endpoint/{endpointId:guid}/series")]
    public async Task<ActionResult<TelemetrySeriesDto>> GetSeries(
        Guid endpointId, [FromQuery] int windowHours = 24)
        => ToActionResult(await _service.GetSeriesAsync(endpointId, windowHours));
}