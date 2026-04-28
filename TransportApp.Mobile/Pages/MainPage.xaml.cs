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

        // Проверяем, что модель привязана и данные еще не загружены (чтобы не спамить запросами)
        if (BindingContext is MainPageModel model)
        {
            // Вызываем команду загрузки из MainPageModel
            // Это заполнит ваш список теми данными S1/R41, которые мы видели на скриншоте
            await model.LoadDeparturesCommand.ExecuteAsync(null);
        }
    }
}