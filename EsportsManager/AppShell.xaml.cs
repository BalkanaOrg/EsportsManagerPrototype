using EsportsManager.Views;

namespace EsportsManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute("teamprofile", typeof(TeamProfileView));
        Routing.RegisterRoute("playerprofile", typeof(PlayerProfileView));
        Routing.RegisterRoute(nameof(TournamentDetailView), typeof(TournamentDetailView));
        Routing.RegisterRoute(nameof(TeamProfileView), typeof(TeamProfileView));
    }
}
