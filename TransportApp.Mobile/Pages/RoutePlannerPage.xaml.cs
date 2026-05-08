namespace TransportApp.Mobile.Pages;

public partial class RoutePlannerPage : ContentPage
{
    public RoutePlannerPage(RoutePlannerPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
