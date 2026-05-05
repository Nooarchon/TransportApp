using TransportApp.Mobile.Models;
using TransportApp.Mobile.PageModels;

namespace TransportApp.Mobile.Pages;

// QueryProperty связывает параметр из навигации со свойством в ЭТОМ классе
[QueryProperty(nameof(SelectedStop), "SelectedStop")]
public partial class StopDetailPage : ContentPage
{
    public StopDeparture? SelectedStop
    {
        // Используем 'SelectedStop' с БОЛЬШОЙ буквы — его создал генератор
        get => (BindingContext as StopDetailPageModel)?.SelectedStop;
        set
        {
            if (BindingContext is StopDetailPageModel vm)
            {
                vm.SelectedStop = value;
            }
        }
    }

    public StopDetailPage(StopDetailPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}