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
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
                    handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<ApiService>();

            // ViewModels
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();
            builder.Services.AddTransient<StopDetailPageModel>();
            builder.Services.AddTransient<RoutePlannerPageModel>();

            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<StopDetailPage>();
            builder.Services.AddTransient<RoutePlannerPage>();
            builder.Services.AddTransient<SchedulePage>();
            builder.Services.AddTransientWithShellRoute<StopDetailPage, StopDetailPageModel>("StopDetailPage");


            // Shell routes
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");


            return builder.Build();
        }
    }
}