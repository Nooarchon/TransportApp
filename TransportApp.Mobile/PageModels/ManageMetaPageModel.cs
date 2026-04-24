using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TransportApp.Mobile.Models;

namespace TransportApp.Mobile.PageModels;

public partial class ManageMetaPageModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Category> categories = new();
    [ObservableProperty] private ObservableCollection<Tag> tags = new();

    [RelayCommand] private async Task Appearing() => await Task.CompletedTask;
    [RelayCommand] private async Task Reset() => await Task.CompletedTask;
    [RelayCommand] private async Task AddCategory() => await Task.CompletedTask;
    [RelayCommand] private async Task SaveCategories() => await Task.CompletedTask;
    [RelayCommand] private async Task DeleteCategory(Category cat) => await Task.CompletedTask;

    // Добавляем то, чего не хватало в логах:
    [RelayCommand] private async Task AddTag() => await Task.CompletedTask;
    [RelayCommand] private async Task SaveTags() => await Task.CompletedTask;
    [RelayCommand] private async Task DeleteTag(Tag tag) => await Task.CompletedTask;
}