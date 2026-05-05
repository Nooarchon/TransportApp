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

    public RoutePlannerPageModel(ApiService api)
    {
        _api = api;
    }

    [ObservableProperty]
    private string _originText;

    [ObservableProperty]
    private string _destinationText;

    [ObservableProperty]
    private bool _isSearching;

    // NotifyPropertyChangedFor заставляет UI пересчитывать видимость списка при изменении коллекции
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOriginSuggestionsVisible))]
    private ObservableCollection<Stop> _originSuggestions = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDestinationSuggestionsVisible))]
    private ObservableCollection<Stop> _destinationSuggestions = new();

    [ObservableProperty]
    private ObservableCollection<Stop> _calculatedRoute = new();

    [ObservableProperty] private Stop _selectedOrigin;
    [ObservableProperty] private Stop _selectedDestination;

    // Упрощенные геттеры
    public bool IsOriginSuggestionsVisible => OriginSuggestions?.Count > 0;
    public bool IsDestinationSuggestionsVisible => DestinationSuggestions?.Count > 0;

    [RelayCommand]
    public async Task SearchOrigin()
    {
        // Проверка: если текст совпадает с уже выбранной остановкой, поиск не нужен
        if (SelectedOrigin != null && OriginText == SelectedOrigin.stop_name) return;

        if (string.IsNullOrWhiteSpace(OriginText) || OriginText.Length < 3)
        {
            OriginSuggestions.Clear();
            OnPropertyChanged(nameof(IsOriginSuggestionsVisible));
            return;
        }

        var results = await _api.SearchStopsAsync(OriginText);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            OriginSuggestions.Clear();
            if (results != null)
            {
                foreach (var stop in results)
                    OriginSuggestions.Add(stop);
            }
            OnPropertyChanged(nameof(IsOriginSuggestionsVisible));
        });
    }

    [RelayCommand]
    public async Task SearchDestination()
    {
        if (SelectedDestination != null && DestinationText == SelectedDestination.stop_name) return;

        if (string.IsNullOrWhiteSpace(DestinationText) || DestinationText.Length < 3)
        {
            DestinationSuggestions.Clear();
            OnPropertyChanged(nameof(IsDestinationSuggestionsVisible));
            return;
        }

        var results = await _api.SearchStopsAsync(DestinationText);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            DestinationSuggestions.Clear();
            if (results != null)
            {
                foreach (var stop in results)
                    DestinationSuggestions.Add(stop);
            }
            OnPropertyChanged(nameof(IsDestinationSuggestionsVisible));
        });
    }

    [RelayCommand]
    public void SelectOrigin(Stop stop)
    {
        if (stop == null) return;

        SelectedOrigin = stop;
        // Обновляем текст через поле, чтобы не вызвать повторный SearchOrigin через сеттер
        _originText = stop.stop_name;
        OnPropertyChanged(nameof(OriginText));

        OriginSuggestions.Clear();
        OnPropertyChanged(nameof(IsOriginSuggestionsVisible));
    }

    [RelayCommand]
    public void SelectDestination(Stop stop)
    {
        if (stop == null) return;

        SelectedDestination = stop;
        _destinationText = stop.stop_name;
        OnPropertyChanged(nameof(DestinationText));

        DestinationSuggestions.Clear();
        OnPropertyChanged(nameof(IsDestinationSuggestionsVisible));
    }

    [RelayCommand]
    public async Task FindRoute()
    {
        if (SelectedOrigin == null || SelectedDestination == null)
        {
            await Shell.Current.DisplayAlert("Info", "Select stops from the list first!", "OK");
            return;
        }

        IsSearching = true;
        try
        {
            var route = await _api.GetShortestRouteAsync(SelectedOrigin.stop_id, SelectedDestination.stop_id);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CalculatedRoute.Clear();
                if (route != null)
                {
                    foreach (var s in route)
                        CalculatedRoute.Add(s);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error finding route: {ex.Message}");
        }
        finally
        {
            IsSearching = false;
        }
    }
}
