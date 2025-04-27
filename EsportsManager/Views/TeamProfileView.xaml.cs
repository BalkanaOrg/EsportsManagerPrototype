using EsportsManager.Models;
using EsportsManager.Services;

namespace EsportsManager.Views;

public partial class TeamProfileView : ContentPage
{
	public TeamProfileView(Team team)
	{
		InitializeComponent();
        BindingContext = new TeamProfileViewModel(DependencyContainer.GameService, team);
    }
}