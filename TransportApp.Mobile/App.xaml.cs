namespace TransportApp.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // This is the modern .NET 9 way to set your starting page
        return new Window(new AppShell());
    }
}