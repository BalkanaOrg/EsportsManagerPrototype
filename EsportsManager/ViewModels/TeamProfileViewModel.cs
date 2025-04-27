using EsportsManager.Models;
using EsportsManager.Views;
using System.Windows.Input;

public class TeamProfileViewModel : BaseViewModel
{
    private Team _team;

    public Team Team
    {
        get => _team;
        set => SetProperty(ref _team, value);
    }

    public ICommand ViewPlayerCommand { get; }

    public TeamProfileViewModel(GameService gameService, Team team) : base(gameService)
    {
        Team = team;
        ViewPlayerCommand = new Command<Player>(ViewPlayer);
    }

    private async void ViewTeam(Team team)
    {
        if (team == null) return;

        var teamVM = new TeamProfileViewModel(_gameService, team);
        await Shell.Current.Navigation.PushAsync(new PlayerProfileView
        {
            BindingContext = teamVM
        });
    }

    private async void ViewPlayer(Player player)
    {
        if (player == null) return;

        await Shell.Current.GoToAsync($"playerprofile?PlayerId={player.Id}");
    }
}