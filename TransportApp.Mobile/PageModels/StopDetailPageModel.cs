using CommunityToolkit.Mvvm.ComponentModel;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.PageModels;
[QueryProperty(nameof(SelectedStop), "SelectedStop")]
public partial class StopDetailPageModel : ObservableObject
{
    [ObservableProperty]
    private StopDeparture? selectedStop;

    partial void OnSelectedStopChanged(StopDeparture value)
    {
        System.Diagnostics.Debug.WriteLine($"=== RECEIVED SelectedStop: {value?.route_short_name} ===");
    }

}