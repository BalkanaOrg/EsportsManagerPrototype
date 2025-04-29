using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    [QueryProperty(nameof(TournamentId), "TournamentId")]
    public partial class TournamentDetailViewModel : BaseViewModel
    {
        private Tournament _tournament;
        public Tournament Tournament
        {
            get => _tournament;
            set => SetProperty(ref _tournament, value);
        }


        private int _tournamentId;
        public int TournamentId
        {
            get => _tournamentId;
            set => SetProperty(ref _tournamentId, value, onChanged: () => LoadTournament(value));
        }

        public ICommand ViewTeamCommand { get; }

        public TournamentDetailViewModel(GameService gameService) : base(gameService)
        {
            ViewTeamCommand = new RelayCommand<Team>(ViewTeam);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tournament: {ex}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
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
    }
}
