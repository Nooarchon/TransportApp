using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.PageModels;

public partial class ProjectListPageModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Project> projects = new();

    [RelayCommand] private async Task Appearing() => await Task.CompletedTask;
    [RelayCommand] private async Task NavigateToProject(Project project) => await Task.CompletedTask;
    [RelayCommand] private async Task AddProject() => await Task.CompletedTask;
}