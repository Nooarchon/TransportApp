using TransportApp.Mobile.Services;

using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Pages;

public partial class SchedulePage : ContentPage
{
    // We removed = new ApiService(). Now the service will come from outside.
    private readonly ApiService _apiService;

    // Add the ApiService parameter to the constructor
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
            // Everything remains the same here
            var departures = await _apiService.GetDeparturesAsync(stopId);
            DeparturesList.ItemsSource = departures;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to retrieve data. Check your API connection.", "OK");
        }
    }
}