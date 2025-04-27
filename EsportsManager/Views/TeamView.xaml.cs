using EsportsManager;
using EsportsManager.Services;
using EsportsManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EsportsManager.Views
{
	public partial class TeamView : ContentPage
	{
		public TeamView()
		{
			InitializeComponent();
            BindingContext = new TeamViewModel(DependencyContainer.GameService);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is TeamViewModel vm)
            {
                vm.LoadData(); // You define this method in the VM
            }
        }
    }
}