using EsportsManager;
using EsportsManager.Services;
using EsportsManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EsportsManager.Views
{
    public partial class StartupPage : ContentPage
    {
        public StartupPage()
        {
            InitializeComponent();
            BindingContext = new StartupViewModel(DependencyContainer.GameService);
        }
    }
}