using CommunityToolkit.Mvvm.ComponentModel;

namespace TransportApp.Mobile.PageModels;

public partial class ProjectDetailPageModel : ObservableObject
{
    [ObservableProperty] private string? name;
}