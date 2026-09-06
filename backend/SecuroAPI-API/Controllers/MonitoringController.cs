using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuroAPI.BusinessLogic.DTO_s.Monitoring;
using SecuroAPI.BusinessLogic.Services.Interfaces;

namespace SecuroAPI_API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MonitoringController: BaseFunctionalController
{

    private readonly IProbeService _probe;

    public MonitoringController(IProbeService probe)
    {
        _probe = probe;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ProbeResult>> Get(
        [FromQuery] string url, CancellationToken ct)
        => Ok(await _probe.ProbeAsync(url, ct));

}