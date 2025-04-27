// TournamentsViewModel.cs
using EsportsManager.Models;
using EsportsManager.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EsportsManager.ViewModels
{
    public class TournamentsViewModel : BaseViewModel
    {
        private ObservableCollection<Tournament> _tournaments;
        private Tournament _selectedTournament;
        private string _currentFilter = "Active";

        public ObservableCollection<Tournament> Tournaments { get; } = new();

        public Tournament SelectedTournament
        {
            get => _selectedTournament;
            set => SetProperty(ref _selectedTournament, value, onChanged: () => ShowTournamentDetails(value));
        }

        public ICommand RefreshCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ViewTournamentCommand { get; }
        public ICommand ViewTeamCommand { get; }

        public TournamentsViewModel(GameService gameService) : base(gameService)
        {
            RefreshTournaments();

            RefreshCommand = new Command(RefreshTournaments);
            FilterCommand = new Command<string>(FilterTournaments);
            ViewTournamentCommand = new Command<Tournament>(ViewTournament);
            ViewTeamCommand = new Command<Team>(ViewTeam);

            LoadTournaments();
        }

        private void RefreshTournaments()
        {
            var state = _gameService.GetGameState();
            FilterTournaments(_currentFilter);
        }

        private void LoadTournaments()
        {
            Tournaments.Clear();
            foreach (var tournament in _gameService.GetActiveTournaments().OrderBy(t=>t.Week).ThenBy(t=>t.Tier))
            {
                Tournaments.Add(tournament);
            }
        }

        private void FilterTournaments(string filter)
        {
            _currentFilter = filter;
            var state = _gameService.GetGameState();

            switch (filter)
            {
                case "Active":
                    _tournaments = new ObservableCollection<Tournament>(state.ActiveTournaments);
                    break;
                case "Upcoming":
                    // Find tournaments that haven't started yet
                    var upcoming = state.ActiveTournaments
                        .Where(t => t.Year > state.CurrentYear ||
                                  (t.Year == state.CurrentYear && t.Week > state.CurrentWeek))
                        .ToList();
                    _tournaments = new ObservableCollection<Tournament>(upcoming);
                    break;
                case "Completed":
                    _tournaments = new ObservableCollection<Tournament>(state.CompletedTournaments);
                    break;
            }
        }

        private async void ViewTournament(Tournament tournament)
        {
            try
            {
                await Shell.Current.GoToAsync($"{nameof(TournamentDetailView)}?TournamentId={tournament.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.ToString()}");
                await Shell.Current.DisplayAlert("Error",
                    $"Couldn't open tournament: {ex.Message}",
                    "OK");
            }
        }

        private async void ViewTeam(Team team)
        {
            if (team == null) return;
            await Shell.Current.Navigation.PushAsync(new TeamProfileView(team));
        }

        private async void ShowTournamentDetails(Tournament tournament)
        {
            if (tournament == null) return;

            // Reset selection
            SelectedTournament = null;

            // Show tournament details
            await Application.Current.MainPage.DisplayAlert(tournament.Name,
                $"Tier: {tournament.Tier}\n" +
                $"Prize Pool: ${tournament.PrizePool:N0}\n" +
                $"Date: Week {tournament.Week} of {tournament.Year}\n" +
                $"Teams: {tournament.ParticipatingTeams.Count}\n" +
                $"Matches Played: {tournament.Matches.Count(m => m.IsCompleted)}/{tournament.Matches.Count}",
                "OK");
        }

        public void LoadData()
        {
            OnPropertyChanged(nameof(Tournament));
            // Or reload whatever data you're binding to
        }
    }
}