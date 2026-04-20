using System;

namespace TransportApp.Mobile.Models;

public class StopDeparture
{
    // The '?' allows these to be null until data is loaded from the API
    public string? trip_id { get; set; }
    public string? route_short_name { get; set; }
    public string? trip_headsign { get; set; }
    public string? arrival_time { get; set; }
}

public class TransportStop
{
    public string? stop_id { get; set; }
    public string? stop_name { get; set; }
    public double stop_lat { get; set; }
    public double stop_lon { get; set; }
}

// Added based on your build warnings for TransportApp.Mobile.Models.Stop
public class Stop
{
    public string? stop_id { get; set; }
    public string? stop_name { get; set; }
    public string? trip_id { get; set; }
    public string? route_short_name { get; set; }
    public string? trip_headsign { get; set; }
    public string? arrival_time { get; set; }
}