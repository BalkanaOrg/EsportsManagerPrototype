// CalendarViewModel.cs
using System.Collections.ObjectModel;
using System.Windows.Input;

public class CalendarViewModel : BaseViewModel
{
    private int _currentYear;
    private int _currentWeek;
    private ObservableCollection<CalendarEvent> _weeklyEvents;

    public int CurrentYear
    {
        get => _currentYear;
        set => SetProperty(ref _currentYear, value);
    }

    public int CurrentWeek
    {
        get => _currentWeek;
        set => SetProperty(ref _currentWeek, value);
    }

    public ObservableCollection<CalendarEvent> WeeklyEvents
    {
        get => _weeklyEvents;
        set => SetProperty(ref _weeklyEvents, value);
    }

    public ICommand NextWeekCommand { get; }

    public CalendarViewModel(GameService gameService) : base(gameService)
    {
        var state = _gameService.GetGameState();
        CurrentYear = state.CurrentYear;
        CurrentWeek = state.CurrentWeek;
        UpdateWeeklyEvents();

        NextWeekCommand = new Command(AdvanceWeek);
    }

    private void UpdateWeeklyEvents()
    {
        var state = _gameService.GetGameState();
        var events = new List<CalendarEvent>();

        // Add tournaments happening this week
        foreach (var tournament in state.ActiveTournaments
            .Where(t => t.Year == CurrentYear &&
                       t.Week <= CurrentWeek &&
                       t.Week + t.DurationWeeks > CurrentWeek))
        {
            events.Add(new CalendarEvent
            {
                Type = "Tournament",
                Description = $"{tournament.Name} ({tournament.Tier})",
                Date = new DateTime(CurrentYear, 1, 1).AddDays((CurrentWeek - 1) * 7)
            });
        }

        // Add matches for user team
        foreach (var match in state.UpcomingMatches
            .Where(m => !m.IsCompleted &&
                       m.MatchDate.Year == CurrentYear &&
                       m.MatchDate.DayOfYear / 7 + 1 == CurrentWeek &&
                       (m.Team1 == state.UserTeam || m.Team2 == state.UserTeam)))
        {
            events.Add(new CalendarEvent
            {
                Type = "Match",
                Description = $"{match.Team1.Name} vs {match.Team2.Name}",
                Date = match.MatchDate
            });
        }

        WeeklyEvents = new ObservableCollection<CalendarEvent>(events);
    }

    private void AdvanceWeek()
    {
        _gameService.ProcessNextWeek();
        var state = _gameService.GetGameState();
        CurrentYear = state.CurrentYear;
        CurrentWeek = state.CurrentWeek;
        UpdateWeeklyEvents();
    }
}

public class CalendarEvent
{
    public string Type { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
}