using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ApprovedUser")]
public class MonitoringController(IMonitoredEndpointService service)
    : BaseFunctionalController
{
    [HttpGet("api/{apiId:guid}")]
    public async Task<ActionResult<IEnumerable<MonitoredEndpointDto>>> GetAllByApi(Guid apiId)
    {
        return ToActionResult(await service.GetAllByAPIID(apiId));
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MonitoredEndpointDto>> GetById(Guid id)
    {
      return ToActionResult(await service.GetByIdAsync(id));
    }
    [HttpPost("api/{apiId:guid}")]
    public async Task<ActionResult<MonitoredEndpointDto>> Create(
        Guid apiId, [FromBody] CreateMonitoredEndpointDto dto)
    {
        return ToActionResult(await service.CreateAsync(apiId, dto));
    }
    [HttpPatch("{id:guid}/active")]
    public async Task<ActionResult<MonitoredEndpointDto>> SetActive(
        Guid id, [FromQuery] bool isActive)
    {
        return ToActionResult(await service.SetActiveAsync(id, isActive));
    }
    
    [HttpPost("{id:guid}/probe")]
    public async Task<ActionResult<ProbeResult>> ProbeNow(Guid id, CancellationToken ct)
    {
        return ToActionResult(await service.ProbeNowAsync(id, ct));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        return ToActionResult(await service.DeleteAsync(id));
    }
}