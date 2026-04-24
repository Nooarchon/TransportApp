using Microsoft.AspNetCore.Mvc;
using TransportApp.Services;

namespace TransportApp.Controllers;

[ApiController]
[Route("api/stops")]
public class StopsController : ControllerBase
{
    private readonly StopsService _service;

    public StopsController(StopsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetStops()
    {
        var stops = await _service.GetStopsAsync();
        return Ok(stops);
    }

    [HttpGet("{id}/departures")]
    public async Task<IActionResult> GetDepartures(string id)
    {
        var deps = await _service.GetDeparturesAsync(id);
        return Ok(deps);
    }

    [HttpGet("route")]
    public IActionResult GetRoute([FromQuery] string from, [FromQuery] string to)
    {
        var path = _service.GetRoute(from, to);
        if (path == null)
            return NotFound("Маршрут не найден");

        return Ok(path);
    }
}
