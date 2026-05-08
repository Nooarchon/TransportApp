using TransportApp.Mobile.PageModels;

namespace TransportApp.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Check that the model is bound and the data hasn't loaded yet (to avoid spamming requests)
        if (BindingContext is MainPageModel model)
        {
            // Call the load command from MainPageModel
            // This will populate your list with the S1/R41 data we saw in the screenshot
            await model.LoadDeparturesCommand.ExecuteAsync(null);
        }
    }
}