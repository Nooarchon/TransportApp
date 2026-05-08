using Npgsql;
using Dapper;

namespace TransportApp;

public class Edge
{
    public string TargetStopId { get; set; } = "";
    public double TravelTimeInSeconds { get; set; }
}

public class StopTimeRow
{
    public string trip_id { get; set; } = "";
    public string stop_id { get; set; } = "";
    public string arrival_time { get; set; } = "";
    public int stop_sequence { get; set; }
}

public class RoutingService
{
    private Dictionary<string, List<Edge>> _graph = new();

    public async Task BuildGraph(NpgsqlConnection conn)
    {
        var sql = @"SELECT trip_id, stop_id, arrival_time, stop_sequence 
                    FROM stop_times 
                    ORDER BY trip_id, stop_sequence";

        var stopTimes = await conn.QueryAsync<StopTimeRow>(sql);
        var grouped = stopTimes.GroupBy(x => x.trip_id);

        foreach (var trip in grouped)
        {
            var orderedStops = trip.ToList();
            for (int i = 0; i < orderedStops.Count - 1; i++)
            {
                var from = orderedStops[i];
                var to = orderedStops[i + 1];

                double weight = CalculateSeconds(from.arrival_time, to.arrival_time);

                if (!_graph.ContainsKey(from.stop_id))
                    _graph[from.stop_id] = new List<Edge>();

                _graph[from.stop_id].Add(new Edge
                {
                    TargetStopId = to.stop_id,
                    TravelTimeInSeconds = weight
                });
            }
        }
        Console.WriteLine("--- Graph of route is made! ---");
    }

    private double CalculateSeconds(string start, string end)
    {
        // Handle GTFS time strings (HH:mm:ss), even if HH > 23
        try
        {
            var s = start.Split(':').Select(int.Parse).ToArray();
            var e = end.Split(':').Select(int.Parse).ToArray();
            var startSec = s[0] * 3600 + s[1] * 60 + s[2];
            var endSec = e[0] * 3600 + e[1] * 60 + e[2];
            return endSec - startSec;
        }
        catch { return 60; }
    }

    public List<string> FindShortestPath(string startId, string endId)
    {
        var distances = new Dictionary<string, double>();
        var previous = new Dictionary<string, string>();
        var priorityQueue = new SortedSet<(double Distance, string StopId)>();

        distances[startId] = 0;
        priorityQueue.Add((0, startId));

        while (priorityQueue.Count > 0)
        {
            var current = priorityQueue.Min;
            priorityQueue.Remove(current);

            if (current.StopId == endId) break;
            if (!_graph.ContainsKey(current.StopId)) continue;

            foreach (var edge in _graph[current.StopId])
            {
                double alt = distances[current.StopId] + edge.TravelTimeInSeconds;
                if (!distances.ContainsKey(edge.TargetStopId) || alt < distances[edge.TargetStopId])
                {
                    priorityQueue.Remove((distances.GetValueOrDefault(edge.TargetStopId, double.PositiveInfinity), edge.TargetStopId));
                    distances[edge.TargetStopId] = alt;
                    previous[edge.TargetStopId] = current.StopId;
                    priorityQueue.Add((alt, edge.TargetStopId));
                }
            }
        }
        return ReconstructPath(previous, endId);
    }



    private List<string> ReconstructPath(Dictionary<string, string> previous, string endId)
    {
        var path = new List<string>();
        var current = endId;
        while (current != null)
        {
            path.Add(current);
            previous.TryGetValue(current, out current);
        }
        path.Reverse();
        return path[0] == endId && path.Count == 1 ? new List<string>() : path;
    }


}