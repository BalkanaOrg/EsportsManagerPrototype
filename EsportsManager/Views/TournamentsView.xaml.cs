using EsportsManager;
using EsportsManager.Models;
using EsportsManager.Services;
using EsportsManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EsportsManager.Views
{
	public partial class TournamentsView : ContentPage
	{
		public TournamentsView()
		{
			InitializeComponent();
            BindingContext = new TournamentsViewModel(DependencyContainer.GameService);
        }
        public void LoadData()
        {
            OnPropertyChanged(nameof(Tournament));
            // Or reload whatever data you're binding to
        }
    }
}