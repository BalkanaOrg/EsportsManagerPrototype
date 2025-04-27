using EsportsManager.Services;
using EsportsManager.Views;

namespace EsportsManager;

public partial class App : Application
{
    public App(GameService gameService)
    {
        InitializeComponent();

        DependencyContainer.GameService = gameService;

        MainPage = new NavigationPage(new StartupPage());
    }
    public void NavigateToMainApp()
    {
        MainPage = new AppShell();
    }
}