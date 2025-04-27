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
            Team = _gameService.GetGameState().UserTeam;

            RefreshCommand = new Command(RefreshTeam);
            BenchPlayerCommand = new Command<Player>(BenchPlayer);
            ActivatePlayerCommand = new Command<Player>(ActivatePlayer);
            ReleasePlayerCommand = new Command<Player>(ReleasePlayer);
        }

        public void RefreshTeam()
        {
            var team = _gameService.GetGameState().UserTeam;
            Team = team;
            team.Budget = _gameService.ReturnBudget();

            ActivePlayers = new ObservableCollection<Player>(team.Players);
            BenchPlayers = new ObservableCollection<Player>(team.Bench);

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
                    _gameService.ReleasePlayer(player);
                    RefreshTeam();
                }
            }
        }

        private async void ShowPlayerProfile(Player player)
        {
            if (player == null) return;

            var vm = new PlayerProfileViewModel(_gameService, player);
            var page = new PlayerProfileView { BindingContext = vm };
            await Application.Current.MainPage.Navigation.PushAsync(page);

            SelectedPlayer = null; // Reset selection
        }
        public void LoadData()
        {
            RefreshTeam();
            OnPropertyChanged(nameof(Team));
            // Or reload whatever data you're binding to
        }
    }
}