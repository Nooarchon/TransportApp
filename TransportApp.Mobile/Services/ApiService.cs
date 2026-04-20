using System;
using System.Net.Http.Json;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;
    // Replace with your actual local IP (e.g., http://192.168.1.50:5000/api)
    private const string BaseUrl = "http://localhost:5000/api/stops";

    public ApiService() => _http = new HttpClient();

    public async Task<List<TransportStop>> GetStopsAsync() =>
        await _http.GetFromJsonAsync<List<TransportStop>>(BaseUrl);

    public async Task<List<StopDeparture>> GetDeparturesAsync(string stopId) =>
        await _http.GetFromJsonAsync<List<StopDeparture>>($"{BaseUrl}/{stopId}/departures");

    public async Task<List<string>> GetRouteAsync(string from, string to) =>
        await _http.GetFromJsonAsync<List<string>>($"{BaseUrl}/route?from={from}&to={to}");
}