using System.Text.Json.Serialization;

namespace TransportApp.Mobile.Models;

public class Stop
{
    [JsonPropertyName("stop_id")]
    public string stop_id { get; set; } = string.Empty;

    [JsonPropertyName("stop_name")]
    public string stop_name { get; set; } = string.Empty;

    [JsonPropertyName("stop_lat")]
    public double stop_lat { get; set; }

    [JsonPropertyName("stop_lon")]
    public double stop_lon { get; set; }
}

public class StopDeparture
{
    [JsonPropertyName("trip_id")]
    public string? trip_id { get; set; }

    [JsonPropertyName("arrival_time")]
    public string? arrival_time { get; set; }

    [JsonPropertyName("departure_time")]
    public string? departure_time { get; set; }

    [JsonPropertyName("route_short_name")]
    public string? route_short_name { get; set; }

    [JsonPropertyName("trip_headsign")]
    public string? trip_headsign { get; set; }
}