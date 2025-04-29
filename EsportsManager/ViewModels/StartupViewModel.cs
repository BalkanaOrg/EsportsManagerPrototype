using EsportsManager;
using System.Windows.Input;

namespace EsportsManager.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private string _teamName;
        private readonly GameService _gameService;

        private double _red = 128;
        private double _green = 128;
        private double _blue = 128;

        public double Red { get => _red; set { SetProperty(ref _red, value); UpdateColor(); } }
        public double Green { get => _green; set { SetProperty(ref _green, value); UpdateColor(); } }
        public double Blue { get => _blue; set { SetProperty(ref _blue, value); UpdateColor(); } }

        private Color _selectedColor = Colors.White;
        public Color SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        public string TeamName
        {
            get => _teamName;
            set => SetProperty(ref _teamName, value);
        }

        private Color _teamColor = Colors.Blue;

        public Color TeamColor
        {
            get => _teamColor;
            set => SetProperty(ref _teamColor, value);
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
            UpdateColor();
            _gameService.InitializeNewGame(TeamName, SelectedColor);

            // Navigate to main app shell
            Application.Current.MainPage = new AppShell();
        }

        private void UpdateColor()
        {
            SelectedColor = Color.FromRgb((byte)Red, (byte)Green, (byte)Blue);
        }
    }
}