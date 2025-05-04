using EsportsManager.Services;
using EsportsManager.Views;
using System.Diagnostics;

namespace EsportsManager;

public partial class App : Application
{
    public App(GameService gameService)
    {
        InitializeComponent();

        DependencyContainer.GameService = gameService;

        try
        {
            MainPage = new NavigationPage(new StartupPage());
        }
        catch
        {

        }
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            // Log the exception details for later debugging
            var exception = (Exception)e.ExceptionObject;
            Debug.WriteLine($"Unhandled exception: {exception.Message}");
        };
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            // Handles background task exceptions
            Console.WriteLine($"[Task Error] {e.Exception.Message}");
            e.SetObserved();
        };
    }
    public void NavigateToMainApp()
    {
        MainPage = new AppShell();
    }
}