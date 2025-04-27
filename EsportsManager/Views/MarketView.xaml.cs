using EsportsManager;
using EsportsManager.Services;
using EsportsManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EsportsManager.Views
{
	public partial class MarketView : ContentPage
	{
		public MarketView()
		{
			InitializeComponent();
            BindingContext = new MarketViewModel(DependencyContainer.GameService);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MarketViewModel vm)
            {
                vm.RefreshCommand?.Execute(null);
            }
        }
    }
}