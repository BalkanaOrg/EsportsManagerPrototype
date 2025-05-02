using CommunityToolkit.Mvvm.ComponentModel;
using EsportsManager.Models;
using EsportsManager.Services;
using System;
using System.Threading.Tasks;

namespace EsportsManager.ViewModels
{
    [QueryProperty(nameof(TeamId), "TeamId")]
    public partial class TeamProfileViewModel : BaseViewModel
    {
        private Team _team;

        public Team Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
        }

        private int _teamId;
        public int TeamId
        {
            get => _teamId;
            set => SetProperty(ref _teamId, value, onChanged: () => LoadTeam(value));
        }

        private readonly GameService _gameService;

        public TeamProfileViewModel(GameService gameService) : base(gameService)
        {
            try
            {
                _gameService = gameService;
            }
            catch
            {

            }
        }

        public async Task LoadTeam(int teamId)
        {
            try
            {
                Team = await _gameService.GetTeamByIdAsync(teamId);
                if (Team == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Team not found", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading team: {ex}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
