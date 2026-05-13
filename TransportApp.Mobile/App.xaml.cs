namespace TransportApp.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Remove "MainPage = new AppShell();" from here
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // This is the single source of truth for the app window in .NET 9
        return new Window(new AppShell());
    }
}