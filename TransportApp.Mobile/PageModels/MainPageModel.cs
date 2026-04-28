using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportApp.Mobile.Models;
using TransportApp.Mobile.Services;

namespace TransportApp.Mobile.PageModels;

public partial class MainPageModel : ObservableObject
{
    private readonly ApiService _api;

    public MainPageModel(ApiService apiService)
    {
        _api = apiService;
        // Загружаем данные при старте
        _ = LoadDepartures();
    }

    [ObservableProperty] private string today = DateTime.Now.ToString("dd MMMM");
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;

    // Используем ID, для которого в базе точно есть записи (T53047)
    [ObservableProperty] private string selectedStopId = "T53047";

    [ObservableProperty]
    private ObservableCollection<StopDeparture> departures = new();

    // Заглушки для UI (чтобы не было ошибок в XAML)
    [ObservableProperty] private ObservableCollection<ProjectTask> tasks = new();
    [ObservableProperty] private bool hasCompletedTasks;

    [RelayCommand]
    public async Task LoadDepartures()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var data = await _api.GetDeparturesAsync(SelectedStopId);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Departures.Clear();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        Departures.Add(item);
                    }
                    System.Diagnostics.Debug.WriteLine($"---> УСПЕХ: В список добавлено {Departures.Count} строк.");
                }
                else
                {
                    // Если API вернул пустой список, показываем одну тестовую строку
                    Departures.Add(new StopDeparture
                    {
                        route_short_name = "INFO",
                        trip_headsign = "Нет рейсов для этой остановки",
                        arrival_time = "--:--"
                    });
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"---> КРИТИЧЕСКАЯ ОШИБКА API: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    // Команды жизненного цикла страницы
    [RelayCommand] private async Task Refresh() => await LoadDepartures();
    [RelayCommand] private async Task Appearing() => await LoadDepartures();

    // Заглушки команд, чтобы UI не выдавал ошибок привязке
    [RelayCommand] private Task NavigatedTo() => Task.CompletedTask;

    // MainPageModel.cs
    [RelayCommand]
    private async Task GoToStopDetails(StopDeparture departure)
    {
        if (departure == null) return;

        // Переходим на страницу деталей, которую мы зарегистрировали в AppShell
        await Shell.Current.GoToAsync(nameof(StopDetailPage), new Dictionary<string, object>
    {
        { "SelectedStop", departure }
    });
    }

    [RelayCommand] private Task NavigatedFrom() => Task.CompletedTask;
    [RelayCommand] private Task AddTask() => Task.CompletedTask;
    [RelayCommand] private Task CleanTasks() => Task.CompletedTask;
    [RelayCommand] private Task TaskCompleted(ProjectTask task) => Task.CompletedTask;
}