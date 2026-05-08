using Dapper;
// REMOVED: using Microsoft.AspNetCore.Mvc;

namespace TransportApp.Services; // Changed namespace to Services

// REMOVED: [ApiController] and [Route] attributes
public class StopsService
{
    private readonly Database _db;
    private readonly RoutingService _routingService;

    public StopsService(Database db, RoutingService routingService)
    {
        _db = db;
        _routingService = routingService;
    }

    // Changed return type from IActionResult to the actual data (IEnumerable<dynamic>)
    public async Task<IEnumerable<dynamic>> GetStopsAsync()
    {
        using var conn = _db.GetConnection();
        var stops = await conn.QueryAsync("SELECT stop_id, stop_name, stop_lat, stop_lon FROM stops");
        return stops; // Removed Ok()
    }

    public async Task<IEnumerable<dynamic>> GetDeparturesAsync(string stopId)
    {
        using var conn = _db.GetConnection();
        var sql = @"SELECT st.trip_id, st.arrival_time, st.departure_time, r.route_short_name, t.trip_headsign
                    FROM stop_times st
                    JOIN trips t ON st.trip_id = t.trip_id
                    JOIN routes r ON t.route_id = r.route_id
                    WHERE st.stop_id = @stopId
                    ORDER BY st.arrival_time LIMIT 30";
        return await conn.QueryAsync(sql, new { stopId }); // Removed Ok()
    }

    public List<string>? GetRoute(string from, string to)
    {
        // Dijkstra search using the Service we built
        var path = _routingService.FindShortestPath(from, to);

        if (path == null || path.Count == 0)
            return null; // Return null instead of NotFound()

        return path;
    }

    public async Task<IEnumerable<dynamic>> GetDetailedRouteAsync(string fromId, string toId)
    {
        var pathIds = _routingService.FindShortestPath(fromId, toId);
        if (pathIds == null || !pathIds.Any()) return Enumerable.Empty<dynamic>();

        using var conn = _db.GetConnection();
        // Fetch names and coordinates for all stops in the calculated path
        var sql = "SELECT stop_id, stop_name, stop_lat, stop_lon FROM stops WHERE stop_id = ANY(@pathIds)";
        var stopDetails = await conn.QueryAsync(sql, new { pathIds = pathIds.ToArray() });

        // Important: Re-sort them to match the Dijkstra path order
        var detailsMap = stopDetails.ToDictionary(s => (string)s.stop_id);
        return pathIds.Select(id => detailsMap[id]);
    }
}