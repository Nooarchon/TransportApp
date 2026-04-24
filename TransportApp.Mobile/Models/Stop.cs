using System.Text.Json.Serialization;

namespace TransportApp.Mobile.Models;

public class StopDeparture
{
    [JsonPropertyName("route_short_name")]
    public string? route_short_name { get; set; } // Добавлен ?

    [JsonPropertyName("trip_headsign")]
    public string? trip_headsign { get; set; } // Добавлен ?

    [JsonPropertyName("arrival_time")]
    public string? arrival_time { get; set; } // Добавлен ?
}

public class Stop
{
    public string? stop_id { get; set; }
    public string? stop_name { get; set; }
    public string? trip_id { get; set; }
    public string? route_short_name { get; set; }
    public string? trip_headsign { get; set; }
    public string? arrival_time { get; set; }
}