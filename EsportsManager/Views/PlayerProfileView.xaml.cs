using EsportsManager;
using EsportsManager.Services;
using EsportsManager.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
namespace EsportsManager.Views
{
    public partial class PlayerProfileView : ContentPage
    {
        public PlayerProfileView()
        {
            InitializeComponent();
        }

        private async void OnSignPlayerClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as PlayerProfileViewModel;
            if (viewModel == null)
                return;

            try
            {
                viewModel.SignPlayerCommand.Execute(null);

                // Now animate the buttons
                await SignButton.FadeTo(0, 250);
                SignButton.IsVisible = false;

                ReleaseButton.Opacity = 0;
                BenchButton.Opacity = 0;
                ReleaseButton.IsVisible = true;
                BenchButton.IsVisible = true;

                await Task.WhenAll(
                    ReleaseButton.FadeTo(1, 250),
                    BenchButton.FadeTo(1, 250)
                );
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void OnReleasePlayerClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as PlayerProfileViewModel;
            if (viewModel == null)
                return;

            try
            {
                viewModel.ReleasePlayerCommand.Execute(null);

                // Animate the buttons back
                await Task.WhenAll(
                    ReleaseButton.FadeTo(0, 250),
                    BenchButton.FadeTo(0, 250)
                );

                ReleaseButton.IsVisible = false;
                BenchButton.IsVisible = false;

                SignButton.Opacity = 0;
                SignButton.IsVisible = true;

                await SignButton.FadeTo(1, 250);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}