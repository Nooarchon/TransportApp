using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportApp.Mobile.Models;
using TransportApp.Mobile.Services;
using TransportApp.Mobile.Pages;

namespace TransportApp.Mobile.PageModels;

public partial class MainPageModel : ObservableObject
{
    private readonly ApiService _api;

    public MainPageModel(ApiService apiService)
    {
        _api = apiService;
        _ = LoadDepartures();
    }

    // Updated date format to English (e.g., 05 May)
    [ObservableProperty] private string _today = DateTime.Now.ToString("dd MMMM");
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _selectedStopId = "T53047";
    [ObservableProperty] private ObservableCollection<StopDeparture> _departures = new();

    [ObservableProperty] private string _searchText;
    [ObservableProperty] private ObservableCollection<Stop> _foundStops = new();
    [ObservableProperty] private bool _isSearching;

    public bool IsSuggestionsVisible => FoundStops?.Count > 0;

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
                    foreach (var item in data) Departures.Add(item);
                }
                else
                {
                    // English placeholder for empty results
                    Departures.Add(new StopDeparture
                    {
                        route_short_name = "INFO",
                        trip_headsign = "No departures for this stop",
                        arrival_time = "--:--"
                    });
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"---> ERROR: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task SearchStops()
    {
        if (string.IsNullOrWhiteSpace(SearchText) || SearchText.Length < 3)
        {
            FoundStops.Clear();
            OnPropertyChanged(nameof(IsSuggestionsVisible));
            return;
        }

        try
        {
            IsSearching = true;
            var results = await _api.SearchStopsAsync(SearchText);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                FoundStops.Clear();
                if (results != null)
                {
                    foreach (var stop in results) FoundStops.Add(stop);
                }
                OnPropertyChanged(nameof(IsSuggestionsVisible));
            });
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    public async Task SelectStop(Stop stop)
    {
        if (stop == null) return;

        SelectedStopId = stop.stop_id;
        SearchText = stop.stop_name;
        FoundStops.Clear();
        OnPropertyChanged(nameof(IsSuggestionsVisible));

        await LoadDepartures();
    }

    [RelayCommand]
    private async Task GoToStopDetails(StopDeparture departure)
    {
        if (departure == null || departure.route_short_name == "INFO") return;

        await Shell.Current.GoToAsync(nameof(StopDetailPage), new Dictionary<string, object>
        {
            { "SelectedStop", departure }
        });
    }

    [RelayCommand] private async Task Refresh() => await LoadDepartures();
    [RelayCommand] private async Task Appearing() => await LoadDepartures();

    // Stubs for XAML compatibility
    [RelayCommand] private Task NavigatedTo() => Task.CompletedTask;
    [RelayCommand] private Task NavigatedFrom() => Task.CompletedTask;
    [ObservableProperty] private ObservableCollection<ProjectTask> _tasks = new();
}