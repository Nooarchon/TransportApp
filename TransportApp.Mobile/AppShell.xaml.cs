using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;
using TransportApp.Mobile.Pages;

namespace TransportApp.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // --- РЕГИСТРАЦИЯ МАРШРУТОВ (Чтобы работала навигация) ---
            Routing.RegisterRoute(nameof(StopDetailPage), typeof(StopDetailPage));
            Routing.RegisterRoute(nameof(RoutePlannerPage), typeof(RoutePlannerPage));

            // Настройка начальной темы
            var currentTheme = Application.Current!.RequestedTheme;

            // Проверка на null для ThemeSegmentedControl, если он объявлен в XAML
            if (ThemeSegmentedControl != null)
            {
                ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
            }
        }

        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#FF3300"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(10), // Сделал чуть более современным
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };

            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static async Task DisplayToastAsync(string message)
        {
            if (OperatingSystem.IsWindows())
            {
                // На Windows лучше использовать Snackbar, так как Toast там часто не виден
                await DisplaySnackbarAsync(message);
                return;
            }

            var toast = Toast.Make(message, textSize: 18);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }

        private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
            }
        }
    }
}