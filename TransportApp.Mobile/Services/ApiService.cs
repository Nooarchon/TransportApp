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
        if (string.IsNullOrEmpty(fromId) || string.IsNullOrEmpty(toId)) return new();

        try
        {
            // Using relative path because BaseAddress is set in constructor
            string url = $"api/stops/route/{Uri.EscapeDataString(fromId)}/{Uri.EscapeDataString(toId)}";

            Debug.WriteLine($"---> Calling: {_http.BaseAddress}{url}");

            // Use a shorter timeout or check for null
            var response = await _http.GetFromJsonAsync<List<Stop>>(url);

            return response ?? new();
        }
        catch (HttpRequestException httpEx)
        {
            Debug.WriteLine($"---> Network/HTTP Error: {httpEx.Message}");
            return new();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> General Route Error: {ex.Message}");
            return new();
        }
    }
}