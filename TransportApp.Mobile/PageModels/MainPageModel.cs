using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportApp.Mobile.Models;
using TransportApp.Mobile.Services;

namespace TransportApp.Mobile.PageModels;

public partial class MainPageModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty] private string today = DateTime.Now.ToString("dd MMMM");
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private string selectedStopId = "U123z1P";
    [ObservableProperty] private ObservableCollection<StopDeparture> departures = new();

    // Свойства-заглушки для совместимости с XAML
    [ObservableProperty] private ObservableCollection<Project> projects = new();
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
            Departures = new ObservableCollection<StopDeparture>(data ?? new());
        }
        finally { IsBusy = false; }
    }

    // Команды, которые ищет XAML в Behaviors
    [RelayCommand] private async Task Refresh() { IsRefreshing = true; await LoadDepartures(); IsRefreshing = false; }
    [RelayCommand] private Task NavigatedTo() => Task.CompletedTask;
    [RelayCommand] private Task NavigatedFrom() => Task.CompletedTask;
    [RelayCommand] private async Task Appearing() => await LoadDepartures();
    [RelayCommand] private Task AddTask() => Task.CompletedTask;
    [RelayCommand] private Task CleanTasks() => Task.CompletedTask;
}