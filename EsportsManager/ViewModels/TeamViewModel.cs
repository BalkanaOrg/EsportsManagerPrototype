// TeamViewModel.cs
using EsportsManager.Models;
using EsportsManager.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EsportsManager.ViewModels
{
    public class TeamViewModel : BaseViewModel
    {
        public ObservableCollection<Player> ActivePlayers { get; set; }
        public ObservableCollection<Player> BenchPlayers { get; set; }
        public ObservableCollection<Match> MatchHistory { get; set; } = new();

        public decimal WeeklyExpense { get; set; }

        private Team _team;
        private Player _selectedPlayer;

        public Team Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
        }

        public Player SelectedPlayer
        {
            get => _selectedPlayer;
            set => SetProperty(ref _selectedPlayer, value, onChanged: () => ShowPlayerProfile(value));
        }

        public ICommand RefreshCommand { get; }
        public ICommand BenchPlayerCommand { get; }
        public ICommand ActivatePlayerCommand { get; }
        public ICommand ReleasePlayerCommand { get; }

        public TeamViewModel(GameService gameService) : base(gameService)
        {
            try
            {
                Team = _gameService.GetGameState().UserTeam;

                RefreshCommand = new Command(RefreshTeam);
                BenchPlayerCommand = new Command<Player>(BenchPlayer);
                ActivatePlayerCommand = new Command<Player>(ActivatePlayer);
                ReleasePlayerCommand = new Command<Player>(ReleasePlayer);
            }
            catch
            {

            }
        }

        private void LoadMatchHistory()
        {
            try
            {
                var allMatches = _gameService.GetAllCompletedMatches(); // however you collect all matches
                var myTeam = Team; // your current team

                var relevantMatches = allMatches
                    .Where(m => m.IsCompleted && (m.Team1 == myTeam || m.Team2 == myTeam))
                    .OrderByDescending(m => m.Year * 100 + m.Week) // latest first
                    .ToList();

                MatchHistory.Clear();
                foreach (var match in relevantMatches)
                    MatchHistory.Add(match);
            }
            catch
            {

            }
        }

        public Color GetResultColor(Match match)
        {
            return match.Winner == Team ? Colors.Green : Colors.Red;
        }

        public void RefreshTeam()
        {
            var team = _gameService.GetGameState().UserTeam;
            Team = team;

            ActivePlayers = new ObservableCollection<Player>(team.Players);
            BenchPlayers = new ObservableCollection<Player>(team.Bench);

            RefreshBudget();
            OnPropertyChanged(nameof(Team));
            OnPropertyChanged(nameof(ActivePlayers)); // <- ADD THIS
            OnPropertyChanged(nameof(BenchPlayers));  // <- AND THIS
        }

        private void BenchPlayer(Player player)
        {
            if (player != null)
            {
                _gameService.MoveToBench(player);
                RefreshTeam();
            }
        }
        private void RefreshBudget()
        {
            var team = _gameService.GetGameState().UserTeam;
            team.Budget = _gameService.ReturnBudget();
            team.WeeklyExpense = _gameService.GetGameState().WeeklyExpense;
            OnPropertyChanged(nameof(Team.WeeklyExpense));
            OnPropertyChanged(nameof(Team.Budget));
        }

        private void ActivatePlayer(Player player)
        {
            if (player != null)
            {

                _gameService.MoveToActive(player);
                RefreshTeam();
            }
        }
        private async void ReleasePlayer(Player player)
        {
            if (player != null)
            {
                bool confirm = await Shell.Current.DisplayAlert("Release Player",$"Are you sure you want to release {player.FullName}?","Yes", "No");
                if (confirm)
                {
                    var team = _gameService.GetGameState().UserTeam;
                    team.WeeklyExpense -= player.Salary;
                    _gameService.ReleasePlayer(player);
                    RefreshTeam();
                }
            }
        }

        private async void ShowPlayerProfile(Player player)
        {
            if (player == null) return;

            var vm = new PlayerProfileViewModel(_gameService, player, true);
            var page = new PlayerProfileView { BindingContext = vm };
            await Application.Current.MainPage.Navigation.PushAsync(page);

            SelectedPlayer = null; // Reset selection
        }
        public void LoadData()
        {
            RefreshTeam();
            OnPropertyChanged(nameof(Team));
            LoadMatchHistory();
            // Or reload whatever data you're binding to
        }
    }
}