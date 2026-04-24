using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.PageModels;

public partial class TaskDetailPageModel : ObservableObject
{
    [ObservableProperty] private string? title;
    [ObservableProperty] private bool isCompleted;
    [ObservableProperty] private bool isExistingProject;
    [ObservableProperty] private Project? project;
    [ObservableProperty] private int selectedProjectIndex;
    [ObservableProperty] private ObservableCollection<Project> projects = new();

    [RelayCommand] private async Task Save() => await Task.CompletedTask;
    [RelayCommand] private async Task Delete() => await Task.CompletedTask;
}