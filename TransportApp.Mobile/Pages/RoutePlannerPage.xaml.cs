namespace TransportApp.Mobile.Pages;

public partial class RoutePlannerPage : ContentPage
{
    public RoutePlannerPage(RoutePlannerPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
