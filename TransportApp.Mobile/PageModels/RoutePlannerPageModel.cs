using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TransportApp.Mobile.Models;
using TransportApp.Mobile.Services;

namespace TransportApp.Mobile.PageModels;

public partial class RoutePlannerPageModel : ObservableObject
{
    private readonly ApiService _api;

    public RoutePlannerPageModel(ApiService apiService)
    {
        _api = apiService;
    }

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
        if (string.IsNullOrWhiteSpace(OriginText) || OriginText.Length < 3)
        {
            IsOriginSuggestionsVisible = false;
            OriginSuggestions.Clear();
            return;
        }

        var results = await _api.SearchStopsAsync(OriginText);
        OriginSuggestions.Clear();
        foreach (var stop in results) OriginSuggestions.Add(stop);

        IsOriginSuggestionsVisible = OriginSuggestions.Count > 0;
        // Reset selection if text changed
        SelectedOrigin = null;
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
        if (stop == null) return;

        // Check your Output window in Visual Studio for this line!
        System.Diagnostics.Debug.WriteLine($"SELECTED: {stop.stop_name}");

        SelectedOrigin = stop;
        OriginText = stop.stop_name;
        IsOriginSuggestionsVisible = false;
    }

    [RelayCommand]
    public void SelectDestination(Stop stop)
    {
        if (stop == null) return;
        SelectedDestination = stop;
        DestinationText = stop.stop_name;
        IsDestinationSuggestionsVisible = false;
        DestinationSuggestions.Clear();
    }

    [RelayCommand]
    public async Task FindRoute()
    {
        // AUTO-MATCH FALLBACK: If user didn't tap, pick the first result from the list
        if (SelectedOrigin == null && OriginSuggestions.Count > 0)
            SelectedOrigin = OriginSuggestions[0];

        if (SelectedDestination == null && DestinationSuggestions.Count > 0)
            SelectedDestination = DestinationSuggestions[0];

        if (SelectedOrigin == null || SelectedDestination == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a valid stop from the results.", "OK");
            return;
        }

        IsSearching = true;
        try
        {
            var route = await _api.GetShortestRouteAsync(SelectedOrigin.stop_id, SelectedDestination.stop_id);
            CalculatedRoute.Clear();
            foreach (var stop in route) CalculatedRoute.Add(stop);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
        finally { IsSearching = false; }

        Debug.WriteLine($"ORIGIN: {SelectedOrigin?.stop_name}");
        Debug.WriteLine($"DEST: {SelectedDestination?.stop_name}");
    }
}