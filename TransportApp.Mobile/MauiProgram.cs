using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using TransportApp.Mobile.PageModels;
using TransportApp.Mobile.Services;
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
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // --- SERVICES ---
            // Fix: Register ApiService WITH HttpClient properly
            builder.Services.AddHttpClient<ApiService>();

            // --- PAGEMODELS ---
            builder.Services.AddTransient<MainPageModel>();
            builder.Services.AddTransient<RoutePlannerPageModel>();
            builder.Services.AddTransient<StopDetailPageModel>();

            // --- PAGES ---
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<RoutePlannerPage>();

            // Shell Route registration
            builder.Services.AddTransientWithShellRoute<StopDetailPage, StopDetailPageModel>("StopDetailPage");

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}