using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TransportApp.Mobile.Models;
using TransportApp.Mobile.Services;

namespace TransportApp.Mobile.PageModels;
// RoutePlannerPageModel.cs

public partial class RoutePlannerPageModel : ObservableObject
{
    private readonly ApiService _api;

    public RoutePlannerPageModel(ApiService apiService)
    {
        _api = apiService;
    }

    public bool HasRoute => CalculatedRoute?.Count > 0;

    [ObservableProperty] private string _originText = string.Empty;
    [ObservableProperty] private string _destinationText = string.Empty;
    [ObservableProperty] private bool _isSearching;

    [ObservableProperty] private Stop? _selectedOrigin;
    [ObservableProperty] private Stop? _selectedDestination;
    [ObservableProperty] private bool _isOriginSuggestionsVisible;
    [ObservableProperty] private bool _isDestinationSuggestionsVisible;

    public ObservableCollection<Stop> OriginSuggestions { get; } = new();
    public ObservableCollection<Stop> DestinationSuggestions { get; } = new();
    public ObservableCollection<Stop> CalculatedRoute { get; } = new();

    [RelayCommand]
    private async Task SearchOrigin()
    {
        // 1. Check if the text is empty/too short
        if (string.IsNullOrWhiteSpace(OriginText) || OriginText.Length < 3)
        {
            IsOriginSuggestionsVisible = false;
            return;
        }

        // 2. AVOID RE-SEARCHING: If the current text already matches the SelectedOrigin, 
        // it means we just picked this from the list, so don't search again.
        if (SelectedOrigin != null && OriginText == SelectedOrigin.stop_name)
        {
            return;
        }

        var results = await _api.SearchStopsAsync(OriginText);

        MainThread.BeginInvokeOnMainThread(() => {
            OriginSuggestions.Clear();
            foreach (var stop in results)
                OriginSuggestions.Add(stop);

            IsOriginSuggestionsVisible = OriginSuggestions.Count > 0;
        });
    }

    [RelayCommand]
    public async Task FindRoute()
    {
        if (SelectedOrigin == null || SelectedDestination == null)
        {
            await Shell.Current.DisplayAlert("Missing Info", "Select both start and end points.", "OK");
            return;
        }

        IsSearching = true;
        try
        {
            // 1. Calls your ApiService method
            var route = await _api.GetShortestRouteAsync(SelectedOrigin.stop_id, SelectedDestination.stop_id);

            // 2. Update the UI collection
            CalculatedRoute.Clear();
            foreach (var stop in route)
            {
                CalculatedRoute.Add(stop);
            }

            // 3. Notify the UI to show the "Recommended Route" section
            OnPropertyChanged(nameof(HasRoute));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"---> Routing Error: {ex.Message}");
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task SearchDestination()
    {
        if (string.IsNullOrWhiteSpace(DestinationText) || DestinationText.Length < 3)
        {
            IsDestinationSuggestionsVisible = false;
            DestinationSuggestions.Clear();
            return;
        }

        var results = await _api.SearchStopsAsync(DestinationText);
        DestinationSuggestions.Clear();
        foreach (var stop in results) DestinationSuggestions.Add(stop);

        IsDestinationSuggestionsVisible = DestinationSuggestions.Count > 0;
        // Reset selection if text changed
        SelectedDestination = null;
    }

    [RelayCommand]
    public void SelectOrigin(Stop stop)
    {
        SelectedOrigin = stop; // FindRoute checks this!
        OriginText = stop.stop_name;
        IsOriginSuggestionsVisible = false;
        OriginSuggestions.Clear();
    }

    [RelayCommand]
    public void SelectDestination(Stop stop)
    {
        if (stop == null) return;

        SelectedDestination = stop;
        DestinationText = stop.stop_name;

        // Hide the list immediately
        DestinationSuggestions.Clear();
        IsDestinationSuggestionsVisible = false;
    }
}