using System.Text.Json.Serialization;

namespace TransportApp.Models
{
    public class PointsOfSaleRoot
    {
        [JsonPropertyName("pointsOfSale")] // This matches the key in the PID JSON
        public List<Point>? points { get; set; }
    }

    public class Point
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}