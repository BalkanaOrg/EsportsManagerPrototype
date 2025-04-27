// PlayerProfileViewModel.cs
using EsportsManager.Models;
using System.Windows.Input;

public class PlayerProfileViewModel : BaseViewModel
{

    public bool IsOwnedByUser => Player?.CurrentTeamId == _gameService.GetGameState().UserTeam.Id;
    public bool IsFreeAgent => Player?.CurrentTeamId == 0;
    public bool IsTeamPlayer => !IsFreeAgent;
    public bool IsActivePlayer => IsTeamPlayer && !Player.IsBenched;
    public bool IsBenchedPlayer => IsTeamPlayer && Player.IsBenched;

    private Player _player;

    public Player Player
    {
        get => _player;
        set
        {
            if (SetProperty(ref _player, value))
            {
                // Whenever Player changes, update dependent computed properties too
                OnPropertyChanged(nameof(IsOwnedByUser));
                OnPropertyChanged(nameof(IsFreeAgent));
                OnPropertyChanged(nameof(IsTeamPlayer));
                OnPropertyChanged(nameof(IsActivePlayer));
                OnPropertyChanged(nameof(IsBenchedPlayer));
            }
        }
    }

    public ICommand SignPlayerCommand { get; }
    public ICommand ReleasePlayerCommand { get; }
    public ICommand BenchPlayerCommand { get; }
    public ICommand ActivatePlayerCommand { get; }

    public PlayerProfileViewModel(GameService gameService, Player player) : base(gameService)
    {
        Player = player;
        var userTeam = _gameService.GetGameState().UserTeam;
        if (player.CurrentTeamId == userTeam.Id)
        {
            OnPropertyChanged(nameof(IsOwnedByUser));
        }
        else
        {
            OnPropertyChanged(nameof(IsOwnedByUser));
        }
        SignPlayerCommand = new Command(SignPlayer);
        ReleasePlayerCommand = new Command(ReleasePlayer);
        BenchPlayerCommand = new Command(BenchPlayer);
        ActivatePlayerCommand = new Command(ActivatePlayer);
    }

    private async void SignPlayer()
    {
        try
        {
            _gameService.SignPlayer(Player, _gameService.GetGameState().UserTeam);
            Player = _player;
        }
        catch (InvalidOperationException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void ReleasePlayer()
    {
        _gameService.ReleasePlayer(Player);
        RefreshPlayerState();
    }

    private void BenchPlayer()
    {
        _gameService.MoveToBench(Player);
        RefreshPlayerState();
    }

    private void ActivatePlayer()
    {
        _gameService.MoveToActive(Player);
        RefreshPlayerState();
    }

    private void RefreshPlayerState()
    {
        Player = _player;
        OnPropertyChanged(nameof(IsOwnedByUser));
        OnPropertyChanged(nameof(IsFreeAgent));
        OnPropertyChanged(nameof(IsTeamPlayer));
        OnPropertyChanged(nameof(IsActivePlayer));
        OnPropertyChanged(nameof(IsBenchedPlayer));
    }
}