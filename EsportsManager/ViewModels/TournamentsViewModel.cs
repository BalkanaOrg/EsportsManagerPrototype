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

        public ObservableCollection<Tournament> Tournaments
        {
            get => _tournaments;
            set => SetProperty(ref _tournaments, value);
        }

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
            _currentFilter = "Active";
            Tournaments = new ObservableCollection<Tournament>(); 
            RefreshCommand = new Command(RefreshTournaments);
            FilterCommand = new Command<string>(FilterTournaments);
            ViewTournamentCommand = new Command<Tournament>(ViewTournament);
            ViewTeamCommand = new Command<Team>(ViewTeam);
            RefreshTournaments();
        }

        private void RefreshTournaments()
        {
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
                    Tournaments = new ObservableCollection<Tournament>(
                        state.ActiveTournaments.OrderBy(t => t.Week).ThenBy(t => t.Tier));
                    break;
                case "Upcoming":
                    // Find tournaments that haven't started yet
                    var upcoming = state.UpcomingTournaments
                        .OrderBy(t => t.Week)
                        .ThenBy(t => t.Tier);
                    Tournaments = new ObservableCollection<Tournament>(upcoming);
                    break;
                case "Completed":
                    Tournaments = new ObservableCollection<Tournament>(state.CompletedTournaments.OrderBy(t => t.Year).ThenBy(t => t.Week));
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

        private async void ShowTournamentDetails(Tournament tournament)
        {
            try
            {
                if (tournament == null) return;
                SelectedTournament = null;
                bool t = tournament.Winner == null;
                if(t)
                {
                    await Application.Current.MainPage.DisplayAlert(tournament.Name,
                        $"Tier: {tournament.Tier}\n" +
                        $"Prize Pool: ${tournament.PrizePool:N0}\n" +
                        $"Date: Week {tournament.Week} of {tournament.Year}\n" +
                        $"Teams: {tournament.ParticipatingTeams.Count}\n" +
                        $"Matches Played: {tournament.Matches.Count(m => m.IsCompleted)}/{tournament.Matches.Count}",
                        "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(tournament.Name,
                        $"Tier: {tournament.Tier}\n" +
                        $"Prize Pool: ${tournament.PrizePool:N0}\n" +
                        $"Date: Week {tournament.Week} of {tournament.Year}\n" +
                        $"Teams: {tournament.ParticipatingTeams.Count}\n" +
                        $"Matches Played: {tournament.Matches.Count(m => m.IsCompleted)}/{tournament.Matches.Count}\n" +
                        $"Winner: {tournament.Winner.Name}",
                        "OK");
                }

                    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.ToString()}");
                await Shell.Current.DisplayAlert("Error",
                    $"Couldn't open tournament: {ex.Message}",
                    "OK");
            }
        }

        public void LoadData()
        {
            OnPropertyChanged(nameof(Tournament));
            // Or reload whatever data you're binding to
        }
    }
}