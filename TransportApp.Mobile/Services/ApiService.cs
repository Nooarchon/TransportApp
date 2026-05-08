using System.Net.Http.Json;
using System.Diagnostics;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Services;

public class ApiService
{
    // Rename to _http to match your field name or _httpClient to match your errors
    // I'll use _http as per your latest code snippet
    private readonly HttpClient _http;

    private static readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                                            ? "http://10.0.2.2:59764/"
                                            : "http://localhost:59764/";

    public ApiService(HttpClient httpClient)
    {
        _http = httpClient;
        _http.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<List<StopDeparture>> GetDeparturesAsync(string stopId)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<StopDeparture>>($"api/stops/{stopId}/departures") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Error getting departures: {ex.Message}");
            return new();
        }
    }

    public async Task<List<Stop>> SearchStopsAsync(string query)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Stop>>($"api/stops/search?name={Uri.EscapeDataString(query)}") ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Error searching stops: {ex.Message}");
            return new();
        }
    }

    public async Task<List<Stop>> GetShortestRouteAsync(string fromId, string toId)
    {
        try
        {
            // FIX 1: Use the parameters fromId and toId to build the URL
            // FIX 2: Do not hardcode "http://localhost..." here because you already set BaseAddress in the constructor
            string encodedFrom = Uri.EscapeDataString(fromId);
            string encodedTo = Uri.EscapeDataString(toId);
            string url = $"api/stops/route/{encodedFrom}/{encodedTo}";

            Debug.WriteLine($"---> Requesting Route: {url}");

            // FIX 3: GetFromJsonAsync handles the BaseAddress + the relative URL
            var response = await _http.GetFromJsonAsync<List<Stop>>(url);
            return response ?? new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Route API Error: {ex.Message}");
            return new();
        }

    }
}