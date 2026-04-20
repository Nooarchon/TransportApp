using TransportApp.Mobile.Models;
using TransportApp.Mobile.PageModels;

namespace TransportApp.Mobile.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            BindingContext = model;
        }
    }
}