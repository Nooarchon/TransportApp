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
        // Временно добавим это, чтобы данные загрузились сразу при создании
        Task.Run(async () => await LoadDepartures());
    }

    [ObservableProperty] private string today = DateTime.Now.ToString("dd MMMM");
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private string selectedStopId = "U123z1P";

    // ОСТАВЛЯЕМ ТОЛЬКО ЭТО. Генератор сам создаст публичное свойство "Departures"
    [ObservableProperty]
    private ObservableCollection<StopDeparture> departures = new();

    // Заглушки (если они нужны для XAML)
    [ObservableProperty] private ObservableCollection<Project> projects = new();
    [ObservableProperty] private ObservableCollection<ProjectTask> tasks = new();
    [ObservableProperty] private bool hasCompletedTasks;

    [ObservableProperty] private ObservableCollection<double> todoCategoryData = new();
    [ObservableProperty] private ObservableCollection<Color> todoCategoryColors = new();

    [RelayCommand]
    private async Task TaskCompleted(ProjectTask task)
    {
        // Логика завершения задачи
        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task LoadDepartures()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var data = await _api.GetDeparturesAsync(SelectedStopId);

            // Работаем в основном потоке интерфейса
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Departures.Clear();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        Departures.Add(item);
                    }
                    // Отладочный вывод: если это появится в Output, значит данные в коде есть
                    System.Diagnostics.Debug.WriteLine($"---> Добавлено {Departures.Count} элементов в коллекцию");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand] private async Task Refresh() => await LoadDepartures();
    [RelayCommand] private async Task Appearing() => await LoadDepartures();

    // Остальные пустые команды для компиляции
    [RelayCommand] private Task NavigatedTo() => Task.CompletedTask;
    [RelayCommand] private Task NavigatedFrom() => Task.CompletedTask;
    [RelayCommand] private Task AddTask() => Task.CompletedTask;
    [RelayCommand] private Task CleanTasks() => Task.CompletedTask;
}