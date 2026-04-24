using TransportApp.Mobile.Services;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Pages;

public partial class SchedulePage : ContentPage
{
    // Мы убрали = new ApiService(). Теперь сервис придет "извне".
    private readonly ApiService _apiService;

    // В конструктор добавляем параметр ApiService
    public SchedulePage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void OnGetScheduleClicked(object sender, EventArgs e)
    {
        var stopId = StopIdEntry.Text?.Trim();
        if (string.IsNullOrEmpty(stopId)) return;

        try
        {
            // Здесь всё остается так же
            var departures = await _apiService.GetDeparturesAsync(stopId);
            DeparturesList.ItemsSource = departures;
        }
        catch (Exception)
        {
            await DisplayAlert("Ошибка", "Не удалось получить данные. Проверьте соединение с API.", "OK");
        }
    }
}