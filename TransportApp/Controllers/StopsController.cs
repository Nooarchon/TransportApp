using Dapper;
using Microsoft.AspNetCore.Mvc;
using TransportApp.Services;

namespace TransportApp.Controllers;

[ApiController]
[Route("api/stops")]
public class StopsController : ControllerBase
{
    private readonly StopsService _service;
    private readonly RoutingService _routingService;
    private readonly Database _db; // 1. Add this field

    // 2. Inject Database into the constructor
    public StopsController(StopsService service, RoutingService routingService, Database db)
    {
        _service = service;
        _routingService = routingService;
        _db = db;
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

    [HttpGet("search")]
    public async Task<IActionResult> SearchStops([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Ok(new List<object>());

        var allStops = await _service.GetStopsAsync();

        var results = allStops
            .Where(s => s.stop_name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .GroupBy(s => s.stop_name)
            .Select(g => new {
                stop_id = g.First().stop_id,
                stop_name = g.First().stop_name
            }) // Explicitly create the object structure
            .Take(10)
            .ToList();

        return Ok(results);
    }

    [HttpGet("route/{fromId}/{toId}")]
    public async Task<IActionResult> GetShortestRoute(string fromId, string toId)
    {
        var pathIds = _routingService.FindShortestPath(fromId, toId);
        if (pathIds == null || !pathIds.Any()) return NotFound();

        using var conn = _db.GetConnection();
        // Using 'dynamic' bypasses the need for the 'Stop' class in the Backend
        var stopsData = await conn.QueryAsync<dynamic>(
            "SELECT stop_id, stop_name, stop_lat, stop_lon FROM stops WHERE stop_id = ANY(@ids)",
            new { ids = pathIds.ToArray() });

        var resultPath = pathIds
            .Select(id => stopsData.FirstOrDefault(s => s.stop_id == id))
            .Where(s => s != null)
            .ToList();

        return Ok(resultPath);
    }
}