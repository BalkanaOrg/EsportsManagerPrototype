using CommunityToolkit.Mvvm.ComponentModel;
using EsportsManager.Models;
using EsportsManager.Services;
using EsportsManager.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public ICommand ViewPlayerCommand;

        private readonly GameService _gameService;

        public TeamProfileViewModel(GameService gameService) : base(gameService)
        {
            try
            {
                _gameService = gameService;
                ViewPlayerCommand = new Command<Player>(ShowPlayerProfile);
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

        private async void ShowPlayerProfile(Player player)
        {
            try
            {
                if (player == null) return;
                await Shell.Current.GoToAsync($"{nameof(TeamProfileView)}?TeamId={player.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to player: {ex}");
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
