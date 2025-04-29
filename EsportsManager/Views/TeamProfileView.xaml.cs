using EsportsManager.Models;
using EsportsManager.Services;
using EsportsManager.ViewModels;

namespace EsportsManager.Views;

public partial class TeamProfileView : ContentPage
{
	public TeamProfileView()
	{
		InitializeComponent();
        BindingContext = new TeamProfileViewModel(DependencyContainer.GameService);
    }
}