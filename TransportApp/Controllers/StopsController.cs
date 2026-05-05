using Microsoft.AspNetCore.Mvc;
using TransportApp.Services;

namespace TransportApp.Controllers;

[ApiController]
[Route("api/stops")]
public class StopsController : ControllerBase
{
    private readonly StopsService _service;
    private readonly RoutingService _routingService; // Добавлено поле

    // Внедряем оба сервиса через конструктор
    public StopsController(StopsService service, RoutingService routingService)
    {
        _service = service;
        _routingService = routingService;
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

        // Фильтруем и убираем дубликаты станций (платформы с одним именем)
        var results = allStops
            .Where(s => s.stop_name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .GroupBy(s => s.stop_name) // Группировка для чистоты UI
            .Select(g => g.First())
            .Take(10)
            .ToList();

        return Ok(results);
    }

    [HttpGet("route/{fromId}/{toId}")]
    public async Task<IActionResult> GetShortestRoute(string fromId, string toId)
    {
        // 1. Поиск кратчайшего пути (ID остановок) через алгоритм Дейкстры
        var pathIds = _routingService.FindShortestPath(fromId, toId);

        if (pathIds == null || !pathIds.Any())
            return NotFound("Маршрут не найден");

        // 2. Обогащаем ID информацией об остановках (именами и т.д.)
        var allStops = await _service.GetStopsAsync();
        var resultPath = pathIds
            .Select(id => allStops.FirstOrDefault(s => s.stop_id == id))
            .Where(s => s != null)
            .ToList();

        return Ok(resultPath);
    }
}