using EsportsManager;
using System.Windows.Input;

namespace EsportsManager.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private string _teamName;
        private readonly GameService _gameService;

        public string TeamName
        {
            get => _teamName;
            set => SetProperty(ref _teamName, value);
        }

        public ICommand CreateTeamCommand { get; }

        public StartupViewModel(GameService gameService) : base(gameService)
        {
            _gameService = gameService;
            CreateTeamCommand = new Command(CreateTeam);
        }

        private async void CreateTeam()
        {
            if (string.IsNullOrWhiteSpace(TeamName))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a team name", "OK");
                return;
            }

            _gameService.InitializeNewGame(TeamName);

            // Navigate to main app shell
            Application.Current.MainPage = new AppShell();
        }
    }
}