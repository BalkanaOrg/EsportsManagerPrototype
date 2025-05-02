using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EsportsManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EsportsManager.Views;

namespace EsportsManager.ViewModels
{
    public class WorldRankingsViewModel : INotifyPropertyChanged
    {
        private readonly GameService _gameService;
        private ObservableCollection<Team> _rankedTeams;
        private Team _selectedTeam;
        private string _searchText;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Team> RankedTeams
        {
            get => _rankedTeams;
            set
            {
                _rankedTeams = value;
                OnPropertyChanged();
            }
        }

        public Team SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                _selectedTeam = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterTeams();
            }
        }
        public ICommand ViewTeamCommand { get; }

        // In constructor:

        public WorldRankingsViewModel(GameService gameService)
        {
            try
            {
                _gameService = gameService;
                ViewTeamCommand = new Command<Team>(ViewTeam);
                LoadRankings();
            }
            catch
            {

            }
        }

        public void LoadRankings()
        {
            var gameState = _gameService.GetGameState();
            _gameService.UpdateTeamRankings();

            // Get teams ordered by their world ranking
            var rankedTeams = gameState.AllTeams
                .OrderBy(t => t.WorldRanking)
                .ToList();

            RankedTeams = new ObservableCollection<Team>(rankedTeams);
        }

        private async void ViewTeam(Team team)
        {
            try
            {
                if (team == null) return;
                await Shell.Current.GoToAsync($"{nameof(TeamProfileView)}?TeamId={team.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to team: {ex}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void FilterTeams()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadRankings();
                return;
            }

            var filtered = _gameService.GetGameState().AllTeams
                .Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.WorldRanking)
                .ToList();

            RankedTeams = new ObservableCollection<Team>(filtered);
        }

        public void RefreshRankings()
        {
            _gameService.UpdateTeamRankings();
            LoadRankings();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
