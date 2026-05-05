using System.Net.Http.Json;
using System.Diagnostics;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;

    // Используем 10.0.2.2 для Android-эмулятора, иначе localhost не сработает
    private static readonly string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                                            ? "http://10.0.2.2:59764/"
                                            : "http://localhost:59764/";

    public ApiService(HttpClient httpClient)
    {
        _http = httpClient;
        _http.BaseAddress = new Uri(BaseUrl);
    }

    // Тот самый отсутствующий метод для получения расписания
    public async Task<List<StopDeparture>> GetDeparturesAsync(string stopId)
    {
        try
        {
            var response = await _http.GetAsync($"api/stops/{stopId}/departures");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<StopDeparture>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Error getting departures: {ex.Message}");
        }
        return new List<StopDeparture>();
    }

    // Метод для поиска остановок (нужен для вашего RoutePlannerPageModel)
    public async Task<List<Stop>> SearchStopsAsync(string query)
    {
        try
        {
            var response = await _http.GetAsync($"api/stops/search?name={Uri.EscapeDataString(query)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Stop>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Error searching stops: {ex.Message}");
        }
        return new List<Stop>();
    }

    // Метод для расчета маршрута (нужен для FindRoute)
    public async Task<List<Stop>> GetShortestRouteAsync(string fromId, string toId)
    {
        try
        {
            var response = await _http.GetAsync($"api/stops/route/{fromId}/{toId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Stop>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Error calculating route: {ex.Message}");
        }
        return new List<Stop>();
    }
}