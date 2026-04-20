using Microsoft.AspNetCore.Mvc;
using TransportApp.Services;

[ApiController]
[Route("api/stops")]
public class StopsController : ControllerBase
{
    private readonly StopsService _stopsService;

    public StopsController(StopsService stopsService) => _stopsService = stopsService;

    [HttpGet]
    public async Task<IActionResult> GetStops() => Ok(await _stopsService.GetStopsAsync());

    [HttpGet("{stopId}/departures")]
    public async Task<IActionResult> GetDepartures(string stopId) => Ok(await _stopsService.GetDeparturesAsync(stopId));

    [HttpGet("route")]
    public IActionResult GetRoute(string from, string to)
    {
        var path = _stopsService.GetRoute(from, to);
        return path != null ? Ok(path) : NotFound("Путь не найден");
    }
}