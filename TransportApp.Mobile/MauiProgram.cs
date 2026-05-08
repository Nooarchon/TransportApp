using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using TransportApp.Mobile.PageModels;
using TransportApp.Mobile.Pages.Controls;
using TransportApp.Mobile.Services;
using TransportApp.Mobile.Utilities;
using TransportApp.Mobile.Pages;

namespace TransportApp.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit();

            // Services
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<ApiService>();

            // ViewModels
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddTransient<RoutePlannerPageModel>();

            // Pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<RoutePlannerPage>();

            // Shell route for StopDetailPage only
            builder.Services.AddTransientWithShellRoute<StopDetailPage, StopDetailPageModel>("StopDetailPage");

            return builder.Build();
        }
    }
}