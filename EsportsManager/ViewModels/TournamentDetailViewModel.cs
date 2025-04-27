using EsportsManager.Models;
using EsportsManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EsportsManager.ViewModels
{
    public class TournamentDetailViewModel : BaseViewModel
    {
        private Tournament _tournament;
        public Tournament Tournament
        {
            get => _tournament;
            set => SetProperty(ref _tournament, value);
        }

        public ICommand ViewTeamCommand { get; }

        public TournamentDetailViewModel(GameService gameService) : base(gameService)
        {
            ViewTeamCommand = new Command<Team>(ViewTeam);
        }

        private async void ViewTeam(Team team)
        {
            if (team == null) return;
            await Shell.Current.Navigation.PushAsync(new TeamProfileView(team));
        }

        public async Task LoadTournament(int tournamentId)
        {
            try
            {
                var tournament = _gameService.GetTournamentById(tournamentId);
                if (tournament == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Tournament not found", "OK");
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                Tournament = tournament;
                OnPropertyChanged(nameof(Tournament));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tournament: {ex}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
