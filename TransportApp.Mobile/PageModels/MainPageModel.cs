using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks; // Ensure this is here
using TransportApp.Mobile.Models;
using TransportApp.Mobile.Services;

namespace TransportApp.Mobile.PageModels;

public partial class MainPageModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty]
    private ObservableCollection<StopDeparture> departures = new();

    [ObservableProperty]
    private string selectedStopId = "U123z1P";

    [ObservableProperty]
    private bool isBusy; // The generator turns this into "IsBusy"

    [ObservableProperty]
    private ObservableCollection<object> todoCategoryData = new();

    [ObservableProperty]
    private ObservableCollection<string> todoCategoryColors = new();

    [RelayCommand]
    public async Task LoadDepartures()
    {
        if (IsBusy) return; // Capital I (the generated property)

        try
        {
            IsBusy = true;
            var data = await _api.GetDeparturesAsync(SelectedStopId);
            if (data != null)
            {
                Departures = new ObservableCollection<StopDeparture>(data);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}