using TransportApp.Mobile.PageModels;

namespace TransportApp.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model; // ВОТ ЭТА СТРОКА КРИТИЧЕСКИ ВАЖНА
    
}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Автоматически загружаем данные при открытии
        if (BindingContext is MainPageModel model)
        {
            await model.LoadDeparturesCommand.ExecuteAsync(null);
        }
    }
}