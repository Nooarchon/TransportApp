using System.Net.Http.Json;
using System.Text.Json;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _http;

    // Для Windows Machine используем localhost. Убедись, что порт 59763 актуален.
    private static string BaseUrl = "https://localhost:59763/";

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
            // Настройка: игнорируем регистр букв в JSON (например, route_short_name vs Route_Short_Name)
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Выполняем запрос
            var result = await _http.GetFromJsonAsync<List<StopDeparture>>($"api/stops/{stopId}/departures", options);

            if (result != null)
            {
                System.Diagnostics.Debug.WriteLine($"---> СЕТЬ: УСПЕХ! Получено {result.Count} записей для остановки {stopId}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("---> СЕТЬ: Сервер вернул пустой результат.");
            }

            return result ?? new List<StopDeparture>();
        }
        catch (Exception ex)
        {
            // Если здесь появится ошибка "Connection refused", значит API не запущено или порт неверный
            System.Diagnostics.Debug.WriteLine($"---> СЕТЬ: КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}");
            return new List<StopDeparture>();
        }
    }
}