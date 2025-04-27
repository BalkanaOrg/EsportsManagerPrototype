using EsportsManager.Services;
using EsportsManager.ViewModels;
namespace EsportsManager.Views;

public partial class CalendarView : ContentPage
{
	public CalendarView()
	{
		InitializeComponent();
        BindingContext = new CalendarViewModel(DependencyContainer.GameService);
    }
}