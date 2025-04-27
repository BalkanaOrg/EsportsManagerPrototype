using EsportsManager.ViewModels;

namespace EsportsManager.Views;

[QueryProperty(nameof(TournamentId), "TournamentId")]
public partial class TournamentDetailView : ContentPage
{
	private readonly TournamentDetailViewModel _viewModel;

	public int TournamentId { get; set; }

	public TournamentDetailView(TournamentDetailViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTournament(TournamentId);
    }
}