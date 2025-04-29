using EsportsManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EsportsManager.Views
{
    public partial class WorldRankingsPage : ContentPage
    {
        public WorldRankingsPage(WorldRankingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is WorldRankingsViewModel vm)
            {
                vm.RefreshRankings();
            }
        }
    }
}