using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.Pages;

[QueryProperty(nameof(SelectedStop), "SelectedStop")]
public partial class StopDetailPage : ContentPage
{
    private StopDeparture? _selectedStop; // Добавьте '?' здесь
    public StopDeparture? SelectedStop
    {
        get => _selectedStop;
        set { _selectedStop = value; OnPropertyChanged(); }
    }

    public StopDetailPage()
    {
        InitializeComponent();
        BindingContext = this;
    }
}