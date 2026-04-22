using System;
using System.Net.Http.Json;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;

    // Автоматический выбор адреса: 10.0.2.2 для Android, localhost для остальных
    private static string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android
                                    ? "http://10.0.2.2:5000/api/stops"
                                    : "http://localhost:5000/api/stops";

    public ApiService()
    {
        // Устанавливаем таймаут, чтобы приложение не зависало вечно при отсутствии связи
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task<List<TransportStop>> GetStopsAsync() =>
        await _http.GetFromJsonAsync<List<TransportStop>>(BaseUrl) ?? new();

    public async Task<List<StopDeparture>> GetDeparturesAsync(string stopId) =>
        await _http.GetFromJsonAsync<List<StopDeparture>>($"{BaseUrl}/{stopId}/departures") ?? new();

    public async Task<List<string>> GetRouteAsync(string from, string to) =>
        await _http.GetFromJsonAsync<List<string>>($"{BaseUrl}/route?from={from}&to={to}") ?? new();
}