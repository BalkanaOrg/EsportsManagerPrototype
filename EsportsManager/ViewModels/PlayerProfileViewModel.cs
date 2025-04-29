// PlayerProfileViewModel.cs
using EsportsManager.Models;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class PlayerProfileViewModel : BaseViewModel
{

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
    public bool IsSigned { get; }

    public bool IsOwnedByUser => Player?.CurrentTeamId == _gameService.GetGameState().UserTeam.Id;
    public bool IsFreeAgent => Player?.CurrentTeamId == 0;
    public bool IsTeamPlayer => !IsFreeAgent;
    public bool IsActivePlayer => IsTeamPlayer && !Player.IsBenched;
    public bool IsBenchedPlayer => IsTeamPlayer && Player.IsBenched;


    public ICommand SignPlayerCommand { get; }
    public ICommand ReleasePlayerCommand { get; }
    public ICommand BenchPlayerCommand { get; }
    public ICommand ActivatePlayerCommand { get; }

    public PlayerProfileViewModel(GameService gameService, Player player, bool isSigned) : base(gameService)
    {
        int id = player.Id;
        var userTeam = _gameService.GetGameState().UserTeam;
        var gameState = _gameService.GetGameState();

        Player = player;
        IsSigned = isSigned;
        if(IsSigned)
        {
            ReleasePlayerCommand = new Command(ReleasePlayer);
            BenchPlayerCommand = new Command(BenchPlayer);
            ActivatePlayerCommand = new Command(ActivatePlayer);
        }
        else
        {
            SignPlayerCommand = new Command(SignPlayer);
        }
            
        
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