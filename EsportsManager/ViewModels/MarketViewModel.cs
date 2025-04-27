// MarketViewModel.cs
using EsportsManager.Models;
using EsportsManager.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EsportsManager.ViewModels
{
    public class MarketViewModel : BaseViewModel
    {
        private string _searchText;
        private ObservableCollection<Player> _players;
        private Player _selectedPlayer;

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value, onChanged: FilterPlayers);
        }

        public ObservableCollection<Player> Players
        {
            get => _players;
            set => SetProperty(ref _players, value);
        }

        public Player SelectedPlayer
        {
            get => _selectedPlayer;
            set => SetProperty(ref _selectedPlayer, value, onChanged: () => ShowPlayerProfile(value));
        }

        public Team Team => _gameService.GetGameState().UserTeam;

        public ICommand RefreshCommand { get; }
        public ICommand SignPlayerCommand { get; }

        public MarketViewModel(GameService gameService) : base(gameService)
        {
            Players = new ObservableCollection<Player>(_gameService.GetGameState().FreeAgents);

            RefreshCommand = new Command(RefreshMarket);
            SignPlayerCommand = new Command<Player>(SignPlayer);

            RefreshMarket();
        }

        private void RefreshMarket()
        {
            System.Diagnostics.Debug.WriteLine("📦 RefreshMarket triggered");

            var agents = _gameService.GetGameState().FreeAgents;
            System.Diagnostics.Debug.WriteLine($"Found {agents.Count} free agents");

            Players = new ObservableCollection<Player>(_gameService.GetGameState().FreeAgents);
            OnPropertyChanged(nameof(Team));
        }

        private void FilterPlayers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                RefreshMarket();
                return;
            }

            var filtered = _gameService.GetGameState().FreeAgents
                .Where(p => p.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                           p.Nationality.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                           p.Role.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Players = new ObservableCollection<Player>(filtered);
        }

        private void SignPlayer(Player player)
        {
            if (player != null)
            {
                try
                {
                    _gameService.SignPlayer(player, Team);
                    RefreshMarket();
                    OnPropertyChanged(nameof(Team));
                }
                catch (InvalidOperationException ex)
                {
                    Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
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
    }
}