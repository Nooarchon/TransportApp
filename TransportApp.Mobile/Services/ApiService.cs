using System.Net.Http.Json;
using System.Text.Json;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;

    // Для Windows Machine используем localhost. Убедись, что порт 59763 актуален.
    private static string BaseUrl = "http://localhost:59764/";

    public ApiService(HttpClient httpClient)
    {
        // Игнорируем ошибки SSL-сертификата для локальной разработки на Windows
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

        _http = new HttpClient(handler);
        _http.BaseAddress = new Uri(BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<List<StopDeparture>> GetDeparturesAsync(string stopId)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"---> ПОПЫТКА ЗАПРОСА: {_http.BaseAddress}api/stops/{stopId}/departures");

            var response = await _http.GetAsync($"api/stops/{stopId}/departures");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"---> СЕРВЕР ОТВЕТИЛ ОШИБКОЙ: {response.StatusCode}");
                return new List<StopDeparture>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<StopDeparture>>();
            System.Diagnostics.Debug.WriteLine($"---> ДАННЫЕ ПОЛУЧЕНЫ! Количество: {result?.Count ?? 0}");
            return result ?? new();
        }
        catch (Exception ex)
        {
            // Если здесь вылетает "HttpRequestException", значит IP или порт недоступны
            System.Diagnostics.Debug.WriteLine($"---> КРИТИЧЕСКАЯ ОШИБКА СЕТИ: {ex.Message}");
            if (ex.InnerException != null)
                System.Diagnostics.Debug.WriteLine($"---> ПРИЧИНА: {ex.InnerException.Message}");
            return new List<StopDeparture>();
        }
    }

}