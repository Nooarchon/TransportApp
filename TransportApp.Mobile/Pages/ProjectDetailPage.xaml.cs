using TransportApp.Mobile.PageModels;

namespace TransportApp.Mobile.Pages;

public partial class ProjectDetailPage : ContentPage
{
    // The 'ProjectDetailPageModel' is injected here by the MAUI service provider
    public ProjectDetailPage(ProjectDetailPage viewModel)
    {
        InitializeComponent();

        // This links the XAML to the ViewModel
        BindingContext = viewModel;
    }
}