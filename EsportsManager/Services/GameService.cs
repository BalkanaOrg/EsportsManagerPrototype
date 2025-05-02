// GameService.cs
using EsportsManager.Models;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.Security.Principal;

public class GameService
{
    private GameState _gameState;
    private Random random = new Random();
    int major1Week = 10;
    int major2Week = 40;
    Tournament major1;
    Tournament major2;

    private readonly List<string> _nationalities = new()
    {
        "Germany", "Bulgaria", "UK", "France", "Spain", "Italy", "Sweden", "Denmark", "Norway", "Finland",
        "Russia", "Ukraine", "Poland", "Turkey", "Brazil", "USA", "Canada", "China", "South Korea", "Australia",
        "Netherlands", "Kazakhstan", "Serbia", "Romania", "Greece", "Albania", "Kosovo", "Mexico", "Argentina",
        "Columbia", "South Africa", "India", "Japan", "Mongolia", "Vietnam", "Indonesia", "Philippines"
    };

    private readonly List<string> _EU = new()
    {
        "Germany", "Bulgaria", "UK", "France", "Spain", "Italy", "Sweden", "Denmark", "Norway", "Finland",
        "Russia", "Ukraine", "Poland", "Turkey", "Netherlands", "Kazakhstan", "Serbia", "Romania", "Greece", "Albania", "Kosovo"
    };

    private readonly List<string> _AM = new()
    {
        "Brazil", "USA", "Canada", "Mexico", "Argentina", "Columbia"
    };

    private readonly List<string> _AS = new()
    {
        "China", "South Korea", "Australia", "South Africa", "India", "Japan", "Mongolia", "Vietnam", "Indonesia", "Philippines"
    };

    private readonly List<Color> _teamColors = new()
    {
        Colors.DarkRed, Colors.Blue, Colors.Green, Colors.DarkViolet, Colors.Purple, Colors.DarkOrange, Colors.Black, Colors.DarkTurquoise, Colors.Aquamarine,
        Colors.DarkKhaki, Colors.Cyan, Colors.Magenta, Colors.Green, Colors.DarkSeaGreen, Colors.DarkGreen,
    };

    public GameService()
    {
        _gameState = new GameState();
    }

    public void InitializeNewGame(string teamName, Color color)
    {
        _gameState = new GameState();

        // Create user team with better starting budget
        decimal budget = 1000000;
        _gameState.UserTeam = new Team
        {
            Id = _gameState.AllTeams.Count + 1,
            Name = teamName,
            Budget = budget,
            Color = color
        };
        _gameState.AllTeams.Add(_gameState.UserTeam);
        _gameState.Budget = budget;

        GenerateAITeams(200);

        GenerateFreeAgents(130);

        GenerateInitialTournaments();

        //AssignInitialTournamentParticipation();
    }

    private void GenerateAITeams(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var aiTeam = new Team
            {
                Id = _gameState.AllTeams.Count + 1,
                Name = GenerateTeamName(),
                Budget = 300000 + random.Next(0, 400000),
                Players = new ObservableCollection<Player>(),
                Bench = new ObservableCollection<Player>(),
                MatchHistory = new List<MatchHistory>(),
                Color = _teamColors[random.Next(_teamColors.Count)],
            };

            for (int j = 0; j < 7; j++)
            {
                var player = GeneratePlayerForTeam(aiTeam.Id, j < 3);
                if (j < 5)
                    aiTeam.Players.Add(player);
                else
                    aiTeam.Bench.Add(player);
            }

            _gameState.AllTeams.Add(aiTeam);
            DefineTeamRegion(aiTeam);
        }
    }
    private Player GeneratePlayerForTeam(int teamId, bool isStarPlayer)
    {
        string nationality = _nationalities[random.Next(_nationalities.Count)];
        var player = PlayerGeneratorService.GeneratePlayer(nationality);
        player.Id = GetNextPlayerId();
        player.CurrentTeamId = teamId;

        if (isStarPlayer)
        {
            player.Stats.Aim = Math.Min(100, player.Stats.Aim + 10);
            player.Stats.Rating = 1.3;
        }
        return player;
    }

    private int GetNextPlayerId()
    {
        return _gameState.AllTeams.SelectMany(t => t.Players.Concat(t.Bench))
               .Concat(_gameState.FreeAgents)
               .DefaultIfEmpty()
               .Max(p => p?.Id ?? 0) + 1;
    }

    public int GetNextTournamentId()
    {
        var existingIds = _gameState.ActiveTournaments
            .Concat(_gameState.CompletedTournaments)
            .Concat(_gameState.UpcomingTournaments)
            .Select(t => t.Id)
            .ToList();

        if (!existingIds.Any()) return 1;

        int maxId = existingIds.Max();
        int nextId = maxId + 1;

        while (existingIds.Contains(nextId))
        {
            nextId++;
        }

        Console.WriteLine($"Generated new tournament ID: {nextId}");
        return nextId;
    }

    private void GenerateFreeAgents(int count)
    {
        _gameState.FreeAgents = new List<Player>();

        for (int i = 0; i < count; i++)
        {
            string nationality = _nationalities[random.Next(_nationalities.Count)];
            var player = PlayerGeneratorService.GeneratePlayer(nationality);
            player.Id = GetNextPlayerId();
            player.CurrentTeamId = 0;

            if (i < 10)
            {
                player.Stats.Rating *= 1.2;
                player.MarketValue *= 1.5m;
            }

            _gameState.FreeAgents.Add(player);
        }
    }

    private void GenerateInitialTournaments()
    {
        //Major
        _gameState.ActiveTournaments = new List<Tournament>();
        int currentYear = _gameState.CurrentYear;

        //var major = CreateTournament(
        //    name: $"{GetCityName()} - {currentYear} Major",
        //    tier: TournamentTier.Major,
        //    prizePool: 3000000,
        //    year: currentYear,
        //    startWeek: 15,
        //    durationWeeks: 3,
        //    teamCount: 32,
        //    format: TournamentFormat.SingleElimination
        //);
        //major.CurrentStage = "Round of 32";
        //_gameState.UpcomingTournaments.Add(major);


        ////Major RMR
        //var rmr = CreateTournament(
        //    name: $"{major.Name} - {currentYear} RMR",
        //    tier: TournamentTier.RMR,
        //    prizePool: 1000000,
        //    year: currentYear,
        //    startWeek: major.Week-5,
        //    durationWeeks: 2,
        //    teamCount: 32,
        //    format: TournamentFormat.SingleElimination
        //);
        //rmr.CurrentStage = "Round of 32";
        //_gameState.UpcomingTournaments.Add(rmr);

        MajorCreator();

        //Random S-tier tournaments
        for (int i = 1; i <= 8; i++)
        {
            var tournament = CreateTournament(
                name: $"{GetTournamentName()} {currentYear} {i}",
                tier: TournamentTier.S,
                prizePool: 250000,
                year: currentYear,
                startWeek: 2 + (i * 3),
                durationWeeks: 5,
                groupstageWeeks: 0,
                playoffWeeks: 0,
                teamCount: 32,
                format: TournamentFormat.SingleElimination
            );
            tournament.CurrentStage = "Round of 32";
            var participants = _gameState.AllTeams
                    .OrderByDescending(t => CalculateTeamStrength(t))
                    .Take(16)
                    .ToList();
            tournament.ParticipatingTeams = participants;
            _gameState.ActiveTournaments.Add(tournament);
        }

        for(int i = 1; i<=2; i++)
        {
            string a;
            if (i == 1) a = "Spring";
            else a = "Fall";
            var tournament = CreateTournament(
                    name: $"ESL Pro League {currentYear} {a}",
                    tier: TournamentTier.A,
                    prizePool: 1000000,
                    year: currentYear,
                    startWeek: 20 * i,
                    durationWeeks: 5,
                    groupstageWeeks: 0,
                    playoffWeeks: 0,
                    teamCount: 32,
                    format: TournamentFormat.SingleElimination
                    );
            tournament.CurrentStage = "Round of 32";
            var participants = _gameState.AllTeams
                    .OrderByDescending(t => CalculateTeamStrength(t))
                    .Take(32)
                    .ToList();
            tournament.ParticipatingTeams = participants;
            _gameState.ActiveTournaments.Add(tournament);
        }
    }

    private Tournament CreateTournament(
        string name,
        TournamentTier tier,
        int prizePool,
        int year,
        int startWeek,
        int durationWeeks,
        int groupstageWeeks,
        int playoffWeeks,
        int teamCount,
        TournamentFormat format)
    {
        var tournament = new Tournament
        {
            Id = GetNextTournamentId(),
            Name = name,
            Tier = tier,
            PrizePool = prizePool,
            Year = year,
            Week = startWeek,
            DurationWeeks = durationWeeks,
            GroupStageWeeks = groupstageWeeks,
            PlayoffWeeks = playoffWeeks,
            ParticipatingTeams = new List<Team>(),
            Matches = new List<Match>(),
            GroupStages = new List<GroupStage>(),
            Format = format,
            IsCompleted = false
        };

        var participants = _gameState.AllTeams
            .OrderByDescending(t => CalculateTeamStrength(t))
            .Take(teamCount)
            .ToList();

        if (!participants.Contains(_gameState.UserTeam))
        {
            participants.RemoveAt(participants.Count - 1);
            participants.Add(_gameState.UserTeam);
        }

        tournament.ParticipatingTeams = participants;

        switch (format)
        {
            case TournamentFormat.SingleElimination:
                GenerateSingleEliminationMatches(tournament);
                break;
            case TournamentFormat.RoundRobin:
                GenerateRoundRobinMatches(tournament);
                break;
        }

        return tournament;
    }

    
    private void AssignInitialTournamentParticipation()
    {
        foreach (var tournament in _gameState.ActiveTournaments)
        {
            var participants = _gameState.AllTeams
                .OrderByDescending(t => CalculateTeamStrength(t))
                .Take(tournament.Tier == TournamentTier.Major ? 32 : 16)
                .ToList();

            if (!participants.Contains(_gameState.UserTeam))
            {
                participants.RemoveAt(participants.Count - 1);
                participants.Add(_gameState.UserTeam);
            }

            tournament.ParticipatingTeams = participants;
        }
    }

    private string GenerateTeamName()
    {
        string[] cities = { "Sofia", "Belgrade", "Bucharest", "Istanbul", "Berlin", "Frankfurt", "Budapest", "Paris", "London", "Edinbourgh", "Liverpool", "Manchester", "Madrid", "Lisbon", "Moscow", "Kiyv", "Stockholm", "Oslo", "Washington", "Los Angeles", "Miami", "Beijing", "Shanghai", "Warsaw", "Ontario", "Austin", "Dallas", "Ankara", "Tokyo", "Astana", "Copenhagen", "Athens", "New Delhi", "Mumbai" };
        string[] prefixes = { "Team", "Esports", "Gaming", "Pro", "Elite", "Prime", "Alpha", "Omega", "Royal", "Noble", "Bulgarian", "German", "English", "French", "British", "Spanish", "Turkish" };
        string[] suffixes = { "Kings", "Legion", "Squad", "Force", "Nation", "Empire", "Dynasty", "Club", "Collective", "Assembly", "Shock", "Defiant", "Eternal", "Hunters", "Charge", "Spark", "Infernal", "Justice", "Spitfire" };
        string[] adjectives = { "Red", "Blue", "Black", "White", "Golden", "Silver", "Dark", "Light", "Mighty", "Fierce", "Unstoppable", "Shocking", "Liquid", "Solid", "Purple", "Pink", "Violet", "Proud", "Cloudy", "Sunny", "Platinum" };
        string[] nouns = { "Wolves", "Eagles", "Dragons", "Lions", "Tigers", "Sharks", "Falcons", "Hawks", "Bears", "Rhinos", "Dolphins", "Swords", "Shields", "Warriors", "Kings", "Might", "Valiance", "Cowboys", "Fuel", "Outlaws", "Giants", "Elfs", "Guns", "Samurais", "Guards" };

        int pattern = random.Next(4);
        return pattern switch
        {
            0 => $"{prefixes[random.Next(prefixes.Length)]} {suffixes[random.Next(suffixes.Length)]}",
            1 => $"{adjectives[random.Next(adjectives.Length)]} {nouns[random.Next(nouns.Length)]}",
            2 => $"{prefixes[random.Next(prefixes.Length)]} {nouns[random.Next(nouns.Length)]}",
            3 => $"{cities[random.Next(cities.Length)]} {suffixes[random.Next(suffixes.Length)]}",
            4 => $"{cities[random.Next(cities.Length)]} {nouns[random.Next(nouns.Length)]}",
            _ => $"{adjectives[random.Next(adjectives.Length)]} {suffixes[random.Next(suffixes.Length)]}"
        };
    }

    private void GenerateTournamentsForYear(int year)
    {
        // Majors (2 per year)
        for (int i = 0; i < 2; i++)
        {
            _gameState.ActiveTournaments.Add(new Tournament
            {
                Id = GetNextTournamentId(),
                Name = $"{GetCityName()} - {year} Major",
                Tier = TournamentTier.Major,
                PrizePool = 1000000,
                Year = year,
                Week = random.Next(10, 40),
                DurationWeeks = 2
            });
        }

        // Tier A tournaments (6 per year)
        for (int i = 0; i < 6; i++)
        {
            _gameState.ActiveTournaments.Add(new Tournament
            {
                Id = GetNextTournamentId(),
                Name = $"{GetTournamentName()} {year}",
                Tier = TournamentTier.S,
                PrizePool = 250000,
                Year = year,
                Week = random.Next(5, 45),
                DurationWeeks = 1
            });
        }
    }

    private string GetCityName()
    {
        string[] cities = { "Berlin", "Paris", "London", "Stockholm", "Los Angeles", "Shanghai", "Seoul", "Moscow", "Sydney", "Rio", "Belgrade", "Budapest", "Sofia", "Beijing", "Copenhagen", "Lisbon" };
        return cities[random.Next(cities.Length)];
    }

    private string GetTournamentName()
    {
        string[] names = { "HackedDreams", "Electronic League", "IEMbg", "BOOM", "EPIC", "Esports World Cup", "Academy", "FACEUP", "StarLadder", "PGL", "Balkana" };
        return names[random.Next(names.Length)];
    }

    public void SignPlayer(Player player, Team team)
    {
        if (team.Budget < player.Salary)
            throw new InvalidOperationException("Not enough budget to sign player");

        if (player.CurrentTeamId == 0)
        {
            _gameState.FreeAgents.Remove(player);
        }
        else
        {
            var previousTeam = _gameState.AllTeams.FirstOrDefault(t => t.Id == player.CurrentTeamId);
            previousTeam?.Players.Remove(player);
            previousTeam?.Bench.Remove(player);
        }

        if (team.Players.Count < 5)
        {
            team.Players.Add(player);
        }
        else
        {
            team.Bench.Add(player);
            player.IsBenched = true;
        }
        _gameState.WeeklyExpense += player.Salary;
        team.WeeklyExpense += player.Salary;
        player.CurrentTeamId = team.Id;
        team.Budget -= player.Buyout;
    }

    public async Task<Team> GetTeamByIdAsync(int teamId)
    {
        var team = _gameState.AllTeams.FirstOrDefault(t => t.Id == teamId);
        return team;
    }

    public void ReleasePlayer(Player player)
    {
        var team = _gameState.AllTeams.FirstOrDefault(t => t.Id == player.CurrentTeamId);
        if (team != null)
        {
            team.WeeklyExpense -= player.Salary;
            _gameState.WeeklyExpense -= player.Salary;
            team.Players.Remove(player);
            team.Bench.Remove(player);
            player.CurrentTeamId = 0;
            player.IsBenched = false;
            _gameState.FreeAgents.Add(player);
        }
    }

    public void MoveToBench(Player player)
    {
        var team = _gameState.AllTeams.FirstOrDefault(t => t.Id == player.CurrentTeamId);
        if (team != null && team.Players.Contains(player))
        {
            team.Players.Remove(player);
            team.Bench.Add(player);
            player.IsBenched = true;
        }
    }

    public void MoveToActive(Player player)
    {
        var team = _gameState.AllTeams.FirstOrDefault(t => t.Id == player.CurrentTeamId);
        if (team != null && team.Bench.Contains(player) && team.Players.Count < 5)
        {
            team.Bench.Remove(player);
            team.Players.Add(player);
            player.IsBenched = false;
        }
    }
    public void MoveToActive(Tournament tournament)
    {
        if (tournament.Year == _gameState.CurrentYear &&
            tournament.Week <= _gameState.CurrentWeek &&
            !_gameState.ActiveTournaments.Contains(tournament))
        {
            _gameState.ActiveTournaments.Add(tournament);
            _gameState.UpcomingTournaments.Remove(tournament);
        }
    }

    public void ProcessNextWeek()
    {
        var tournamentsThisWeek = _gameState.ActiveTournaments
            .Where(t => t.Year == _gameState.CurrentYear &&
                        t.Week <= _gameState.CurrentWeek &&
                        t.Week + t.DurationWeeks > _gameState.CurrentWeek)
            .ToList();
        foreach (var tournament in tournamentsThisWeek)
        {
            ProcessTournamentWeek(tournament);
        }

        _gameState.CurrentWeek++;
        if (_gameState.CurrentWeek > 52)
        {
            _gameState.CurrentWeek = 1;
            _gameState.CurrentYear++;

            GenerateTournamentsForYear(_gameState.CurrentYear);
        }

        // RMR logic
        if (_gameState.CurrentWeek == major1.Week - 6)
        {
            var rmr = _gameState.ActiveTournaments
                .Concat(_gameState.CompletedTournaments)
                .FirstOrDefault(t => t.RelatedTournamentId == major1.Id);
            PopulateRMR(rmr);
        }
        else if (_gameState.CurrentWeek == major2.Week - 6)
        {
            var rmr = _gameState.ActiveTournaments
                .Concat(_gameState.CompletedTournaments)
                .FirstOrDefault(t => t.RelatedTournamentId == major2.Id);
            PopulateRMR(rmr);
        }

        if (_gameState.CurrentWeek == major1.Week - 1)
        {
            var rmr = _gameState.ActiveTournaments
                .Concat(_gameState.CompletedTournaments)
                .FirstOrDefault(t => t.RelatedTournamentId == major1.Id);
            ConcludeRMR(rmr);
        }
        else if (_gameState.CurrentWeek == major2.Week - 1)
        {
            var rmr = _gameState.ActiveTournaments
                .Concat(_gameState.CompletedTournaments)
                .FirstOrDefault(t => t.RelatedTournamentId == major2.Id);
            ConcludeRMR(rmr);
        }

        _gameState.Budget -= _gameState.WeeklyExpense;
        UpdateTeamRankings();
    }

    private void ProcessTournamentWeek(Tournament tournament)
    {
        if (tournament.IsCompleted)
            return;

        bool isSingleWeek = tournament.DurationWeeks == 1;

        // 1. Play matches scheduled for this week and stage
        var matchesToPlay = tournament.Matches
            .Where(m =>
                m.Stage == tournament.CurrentStage &&
                !m.IsCompleted &&
                (ShouldPlayMatchThisWeek(m.Year, m.Week) || isSingleWeek))
            .ToList();

        foreach (var match in matchesToPlay)
        {
            PlayMatch(match);
        }

        // 2. Check if the current stage is complete
        bool stageComplete = tournament.Matches
            .Where(m => m.Stage == tournament.CurrentStage)
            .All(m => m.IsCompleted);

        // 3. Advance stage if complete
        if (stageComplete)
        {
            AdvanceTournamentStage(tournament);

            // 4. For single-week, immediately process the next stage
            if (!tournament.IsCompleted && isSingleWeek)
            {
                ProcessTournamentWeek(tournament);
            }
        }
    }


    private void ProcessGroupStage(Tournament tournament)
    {
        if (!tournament.HasGroupStage) return;

        foreach (var group in tournament.GroupStages)
        {
            group.CalculateStandings();
        }

        bool allGroupsComplete = tournament.GroupStages
            .All(g => g.Matches.All(m => m.IsCompleted));

        if (allGroupsComplete && !tournament.Matches.Any())
        {
            GeneratePlayoffMatches(tournament);
        }
    }

    private void GenerateRoundRobinMatches(Tournament tournament)
    {
        tournament.Matches.Clear();
        tournament.GroupStages.Clear();

        int teamCount = tournament.ParticipatingTeams.Count;
        int groupSize = teamCount switch
        {
            < 16 => 4,
            < 32 => 8,
            _ => 16
        };

        int groupCount = (int)Math.Ceiling((double)teamCount / groupSize);

        // Create groups
        var shuffledTeams = tournament.ParticipatingTeams
            .OrderBy(t => Guid.NewGuid())
            .ToList();

        for (int i = 0; i < groupCount; i++)
        {
            var group = new GroupStage
            {
                Name = $"Group {(char)('A' + i)}",
                Teams = shuffledTeams.Skip(i * groupSize).Take(groupSize).ToList()
            };

            GenerateRoundRobinMatchesForGroup(group);
            tournament.GroupStages.Add(group);
        }

        tournament.TeamsPerGroup = groupSize;
        tournament.TeamsAdvancingPerGroup = Math.Max(2, groupSize / 2);
    }

    private void GenerateRoundRobinMatchesForGroup(GroupStage group)
    {
        for (int i = 0; i < group.Teams.Count; i++)
        {
            for (int j = i + 1; j < group.Teams.Count; j++)
            {
                group.Matches.Add(new Match
                {
                    Team1 = group.Teams[i],
                    Team2 = group.Teams[j],
                    Stage = group.Name,
                    MatchDate = CalculateGroupStageMatchDate(group.Name, i, j),
                    IsDeciderMatch = false
                });
            }
        }
    }

    private double CalculateMomentumFactor(Team team1, Team team2)
    {
        var team1Recent = GetRecentWins(team1, 5);
        var team2Recent = GetRecentWins(team2, 5);
        var headToHead = GetHeadToHeadRecord(team1, team2);

        double momentum = 1.0;
        momentum += (team1Recent.Wins - team2Recent.Wins) * 0.05;
        momentum += (headToHead.Team1Wins - headToHead.Team2Wins) * 0.03;

        return Math.Clamp(momentum, 0.8, 1.2);
    }


    private void AdvanceTournamentStage(Tournament tournament)
    {
        string currentStage = tournament.CurrentStage;

        var currentWinners = tournament.Matches
            .Where(m => m.Stage == currentStage)
            .Select(m => m.Winner)
            .Where(w => w != null)
            .ToList();

        // If only one team remains, they are the champion
        if (currentWinners.Count == 1)
        {
            CompleteTournament(tournament);
            return;
        }

        string nextStage = GetNextStage(currentStage);
        int nextStageNumber = GetStageNumber(nextStage); // e.g., Round of 16 = 3

        for (int i = 0; i < currentWinners.Count; i += 2)
        {
            if (i + 1 < currentWinners.Count)
            {
                var match = new Match
                {
                    Team1 = currentWinners[i],
                    Team2 = currentWinners[i + 1],
                    Tournament = tournament,
                    Year = tournament.Year,
                    Week = CalculateTournamentMatchDate(tournament, nextStageNumber),
                    Stage = nextStage,
                    IsDeciderMatch = (nextStage == "Grand Final")
                };
                tournament.Matches.Add(match);
            }
            else
            {
                // Bye or odd team – award partial prize maybe
                currentWinners[i].Budget += tournament.PrizePool * 0.05m;
            }
        }

        tournament.CurrentStage = nextStage;
    }

    private void AdvanceSingleElimination(Tournament tournament)
    {
        string currentStage = tournament.CurrentStage;

        var currentWinners = tournament.Matches
            .Where(m => m.Stage == currentStage)
            .Select(m => m.Winner)
            .ToList();

        if (currentWinners.Count == 1)
        {
            CompleteTournament(tournament);
            return;
        }

        string nextStage = GetNextStage(currentStage);

        for (int i = 0; i < currentWinners.Count; i += 2)
        {
            if (i + 1 < currentWinners.Count)
            {
                tournament.Matches.Add(new Match
                {
                    Team1 = currentWinners[i],
                    Team2 = currentWinners[i + 1],
                    Tournament = tournament,
                    MatchDate = CalculateMatchDate(tournament, GetStageNumber(nextStage)),
                    Stage = nextStage,
                    IsDeciderMatch = (nextStage == "Grand Final")
                });
            }
            else
            {
                // Handle odd number of winners (shouldn't happen in single elim)
                currentWinners[i].Budget += tournament.PrizePool * 0.05m;
            }
        }

        tournament.CurrentStage = nextStage;
    }

    private int GetStageRoundNumber(string stage)
    {
        if (stage.StartsWith("Round of"))
        {
            if (int.TryParse(stage.Replace("Round of", "").Trim(), out int number))
            {
                return number;
            }
        }
        else if (stage == "Semifinals")
        {
            return 4;
        }
        else if (stage == "Final" || stage == "Grand Final")
        {
            return 2;
        }

        return 0;
    }

    private int GetStageNumber(string stage)
    {
        return stage switch
        {
            "Round of 32" => 1,
            "Round of 16" => 2,
            "Quarterfinals" => 3,
            "Semifinals" => 4,
            "Grand Final" => 5,
            _ => 1 // Default to first stage
        };
    }

    private string GetNextStage(string currentStage)
    {
        return currentStage switch
        {
            "Round of 64" => "Round of 32",
            "Round of 32" => "Round of 16",
            "Round of 16" => "Quarterfinals",
            "Quarterfinals" => "Semifinals",
            "Semifinals" => "Grand Final",
            _ => "Finals"
        };
    }

    private void GenerateInitialMatches(Tournament tournament)
    {
        tournament.Matches.Clear();

        switch (tournament.Format)
        {
            case TournamentFormat.SingleElimination:
                GenerateSingleEliminationMatches(tournament);
                break;

            case TournamentFormat.DoubleElimination:
                GenerateDoubleEliminationMatches(tournament);
                break;

            case TournamentFormat.RoundRobin:
                GenerateRoundRobinMatches(tournament);
                break;

            //case TournamentFormat.Swiss:
            //    GenerateSwissMatches(tournament);
            //    break;
        }
    }
    private void GenerateNextRoundMatches(Tournament tournament)
    {
        var winners = tournament.Matches
            .Where(m => m.IsCompleted)
            .Select(m => m.Winner)
            .Distinct()
            .OrderBy(t => Guid.NewGuid())
            .ToList();

        tournament.Matches.Clear();

        for (int i = 0; i < winners.Count; i += 2)
        {
            if (i + 1 < winners.Count)
            {
                tournament.Matches.Add(new Match
                {
                    Team1 = winners[i],
                    Team2 = winners[i + 1],
                    Tournament = tournament,
                    MatchDate = new DateTime(_gameState.CurrentYear, 1, 1).AddDays((_gameState.CurrentWeek - 1) * 7)
                });
            }
            else
            {
                winners[i].Budget += tournament.PrizePool * 0.1m;
            }
        }
    }

    private void PlayMatch(Match match)
    {
        if (match.Team1 == null) return; // Bye match
        if (match.Team2 == null) return; // Bye match

        double team1Strength = CalculateTeamStrength(match.Team1) * GetMapPerformanceFactor(match.Team1, GetRandomMap());
        double team2Strength = CalculateTeamStrength(match.Team2) * GetMapPerformanceFactor(match.Team2, GetRandomMap());

        // Apply momentum & clutch
        double momentum = CalculateMomentumFactor(match.Team1, match.Team2);
        team1Strength *= momentum;
        team2Strength *= (2 - momentum);

        double clutchFactor = 1 + (random.NextDouble() * 0.2 - 0.1);
        double totalStrength = team1Strength + team2Strength;

        // Simulate regulation rounds (first to 13)
        int team1Rounds = 0;
        int team2Rounds = 0;

        while (team1Rounds < 13 && team2Rounds < 13)
        {
            double chance = random.NextDouble() * totalStrength;
            if (chance < team1Strength) team1Rounds++;
            else team2Rounds++;
        }

        // If both reached 12 before one got to 13, trigger OT
        if (team1Rounds == 12 && team2Rounds == 12)
        {
            team1Rounds = 12;
            team2Rounds = 12;
            bool hasWinner = false;

            int ot1 = 0, ot2 = 0;

            while (!hasWinner)
            {
                ot1 = 0;
                ot2 = 0;

                for (int i = 0; i < 6; i++)
                {
                    double chance = random.NextDouble() * totalStrength;
                    if (chance < team1Strength) ot1++;
                    else ot2++;
                }

                if (Math.Abs(ot1 - ot2) >= 2 && (ot1 >= 4 || ot2 >= 4))
                {
                    hasWinner = true;
                }
            }

            team1Rounds += ot1;
            team2Rounds += ot2;
        }

        match.Team1Score = team1Rounds;
        match.Team2Score = team2Rounds;
        match.IsCompleted = true;
        match.Winner = team1Rounds > team2Rounds ? match.Team1 : match.Team2;

        GeneratePlayerPerformances(match);
        UpdateTeamMomentum(match);
    }


    private void UpdateTeamMomentum(Match match)
    {
        match.Winner.Momentum = Math.Min(1.2, match.Winner.Momentum * 1.05);
        Team loser = match.Winner == match.Team1 ? match.Team2 : match.Team1;
        loser.Momentum = Math.Max(0.8, loser.Momentum * 0.95);
    }

    private void GenerateRealisticPlayerPerformances(Match match)
    {
        // Team1 players
        foreach (var player in match.Team1.Players)
        {
            double performanceFactor = (player.Stats.Rating / 2.0) * (random.NextDouble() * 0.4 + 0.8);

            player.MatchHistory.Add(new MatchHistory
            {
                PlayerId = player.Id,
                MatchId = match.Id,
                Kills = (int)(15 * performanceFactor),
                Deaths = (int)(20 * (1.3 - performanceFactor)),
                Assists = (int)(8 * performanceFactor),
                Rating = Math.Round(performanceFactor * 1.5, 2),
                ADR = (int)(80 * performanceFactor),
                FlashAssists = random.Next(0, 5),
                EntryKills = random.Next(0, 4)
            });
        }

        // Team2 players (same logic)
        foreach (var player in match.Team2.Players)
        {
            // ... similar implementation ...
        }
    }

    private void GeneratePlayerMatchHistories(Match match)
    {
        foreach (var player in match.Team1.Players.Concat(match.Team2.Players))
        {
            player.MatchHistory.Add(new MatchHistory
            {
                PlayerId = player.Id,
                MatchId = match.Id,
                Kills = random.Next(5, 30),
                Deaths = random.Next(5, 30),
                Assists = random.Next(0, 15),
                Rating = random.NextDouble() * 2.0,
                ADR = random.Next(50, 120),
                FlashAssists = random.Next(0, 5),
                EntryKills = random.Next(0, 5)
            });
        }
    }

    private Team DetermineTournamentWinner(Tournament tournament)
    {
        // Simple implementation - team with most match wins
        return tournament.Matches
            .GroupBy(m => m.Winner)
            .OrderByDescending(g => g.Count())
            .First().Key;
    }

    private void DistributePrizeMoney(Tournament tournament)
    {
        // Simple prize distribution
        if (tournament.Winner != null)
        {
            tournament.Winner.Budget += tournament.PrizePool * 0.5m;

            // Distribute remaining to other teams
            decimal remaining = tournament.PrizePool * 0.5m;
            decimal perTeam = remaining / (tournament.ParticipatingTeams.Count - 1);

            foreach (var team in tournament.ParticipatingTeams.Where(t => t != tournament.Winner))
            {
                team.Budget += perTeam;
            }
        }
    }
    public void VerifyGameState()
    {
        Console.WriteLine("=== GAME STATE VERIFICATION ===");
        Console.WriteLine($"Teams: {_gameState.AllTeams.Count}");
        Console.WriteLine($"Free Agents: {_gameState.FreeAgents.Count}");
        Console.WriteLine($"Tournaments: {_gameState.ActiveTournaments.Count}");

        foreach (var t in _gameState.ActiveTournaments)
        {
            Console.WriteLine($"- {t.Name}: {t.ParticipatingTeams.Count} teams");
        }

        Console.WriteLine("User Team Players: " + _gameState.UserTeam.Players.Count);
        Console.WriteLine("First AI Team Players: " + _gameState.AllTeams[1].Players.Count);
        Console.WriteLine("Top Free Agent: " + (_gameState.FreeAgents.FirstOrDefault()?.FullName ?? "None"));
    }

    public List<Player> GetFreeAgents()
    {
        return _gameState.FreeAgents
            .OrderByDescending(p => p.Stats.Rating)
            .ToList();
    }

    public List<Tournament> GetActiveTournaments()
    {
        return _gameState.ActiveTournaments
            .Where(t => t.Year == _gameState.CurrentYear &&
                       t.Week + t.DurationWeeks >= _gameState.CurrentWeek)
            .OrderBy(t => t.Week)
            .ToList();
    }

    public List<Team> GetOpponentsForTeam(Team team)
    {
        return _gameState.ActiveTournaments
            .SelectMany(t => t.ParticipatingTeams)
            .Where(t => t != team)
            .Distinct()
            .ToList();
    }

    public List<Match> GetUpcomingMatches(Team team)
    {
        return _gameState.ActiveTournaments
            .SelectMany(t => t.Matches)
            .Where(m => !m.IsCompleted &&
                       (m.Team1 == team || m.Team2 == team))
            .OrderBy(m => m.MatchDate)
            .ToList();
    }

    public Tournament? GetTournamentById(int tournamentId)
    {
        return _gameState.ActiveTournaments
            .Concat(_gameState.UpcomingTournaments)
            .Concat(_gameState.CompletedTournaments)
            .FirstOrDefault(t => t.Id == tournamentId);
    }

    public void UpdateTeamRankings()
    {
        var allTeams = _gameState.AllTeams;
        var allMatches = GetAllCompletedMatches();

        // Calculate team scores with the new algorithm
        foreach (var team in allTeams)
        {
            DefineTeamRegion(team);
        }

        var teamScores = allTeams.Select(team =>
        {
            double compositionScore = CalculateTeamCompositionScore(team);
            double strengthScore = CalculateTeamStrength(team);
            double performanceScore = CalculateRecentPerformance(team, allMatches);

            return new
            {
                Team = team,
                TotalScore = compositionScore * strengthScore * performanceScore
            };
        })
        .OrderByDescending(t => t.TotalScore)
        .ToList();

        // Assign rankings based on the new scores
        for (int i = 0; i < teamScores.Count; i++)
        {
            teamScores[i].Team.WorldRanking = i + 1;
            teamScores[i].Team.TeamScore = teamScores[i].TotalScore;
        }
    }

    private List<Match> GetAllCompletedMatches()
    {
        return _gameState.CompletedTournaments
            .SelectMany(t => t.Matches)
            .Where(m => m.IsCompleted)
            .ToList();
    }

    private double CalculateTeamCompositionScore(Team team)
    {
        // Check for required roles
        var roles = team.Players.Select(p => p.Role).ToList();

        bool hasRifler = roles.Contains(PlayerRole.Rifler);
        bool hasAWPer = roles.Contains(PlayerRole.AWPer);
        bool hasIGL = roles.Contains(PlayerRole.IGL);
        bool hasSupport = roles.Contains(PlayerRole.Support);
        bool hasLurker = roles.Contains(PlayerRole.Lurker);

        // Base composition score (1.0 = perfect composition)
        double compositionScore = 1.0;

        if (!hasRifler) compositionScore *= 0.85;
        if (!hasAWPer) compositionScore *= 0.80;
        if (!hasIGL) compositionScore *= 0.70; 
        if (!hasSupport) compositionScore *= 0.90;
        if (!hasLurker) compositionScore *= 0.95;

        // Bonus for having all roles
        if (hasRifler && hasAWPer && hasIGL && hasSupport && hasLurker)
            compositionScore *= 1.10;

        return compositionScore;
    }

    private double CalculateTeamStrength(Team team)
    {
        if (team.Players.Count == 0) return 0;

        double totalStrength = 0;
        double roleSynergyBonus = 1.0;
        var roles = team.Players.Select(p => p.Role).ToList();

        foreach (var player in team.Players)
        {
            double playerStrength = player.Stats.Rating;

            switch (player.Role)
            {
                case PlayerRole.IGL:
                    playerStrength *= 1 + (player.Stats.GameSense * 0.05);
                    break;
                case PlayerRole.AWPer:
                    playerStrength *= 1 + (player.Stats.Aim * 0.03 - player.Stats.Reflexes * 0.01);
                    break;
                case PlayerRole.Support:
                    playerStrength *= 1 + (player.Stats.Teamwork * 0.04);
                    break;
                case PlayerRole.Lurker:
                    playerStrength *= 1 + (player.Stats.Consistency * 0.04);
                    break;
                case PlayerRole.Rifler:
                    playerStrength *= 1 + (player.Stats.Aim * 0.04);
                    break;
            }

            totalStrength += playerStrength;
        }

        if (roles.Contains(PlayerRole.IGL) && roles.Contains(PlayerRole.Support))
            roleSynergyBonus *= 1.15;

        if (roles.Count(r => r == PlayerRole.Rifler) >= 2)
            roleSynergyBonus *= 1.05;

        double averageStrength = totalStrength / team.Players.Count;
        return averageStrength * roleSynergyBonus;
    }

    private double CalculateRecentPerformance(Team team, List<Match> allMatches)
    {
        var teamMatches = allMatches
            .Where(m => m.Team1 == team || m.Team2 == team)
            .OrderByDescending(m => m.MatchDate)
            .Take(10)
            .ToList();

        if (!teamMatches.Any()) return 1.0;

        int wins = 0;
        int matches = 0;
        double totalPerformance = 0;

        foreach (var match in teamMatches)
        {
            bool isTeam1 = match.Team1 == team;
            bool won = isTeam1 ? match.Team1Score > match.Team2Score : match.Team2Score > match.Team1Score;

            if (won) wins++;
            matches++;

            double performance = isTeam1 ?
                (double)match.Team1Score / (match.Team1Score + match.Team2Score) :
                (double)match.Team2Score / (match.Team1Score + match.Team2Score);

            totalPerformance += performance;
        }

        double winRate = (double)wins / matches;
        double avgPerformance = totalPerformance / matches;

        return (winRate * 0.6) + (avgPerformance * 0.4);
    }

    private void GenerateSingleEliminationMatches(Tournament tournament)
    {
        tournament.Matches.Clear();

        // Ensure we have teams to create matches
        if (tournament.ParticipatingTeams == null || tournament.ParticipatingTeams.Count < 2)
        {
            Console.WriteLine("Not enough teams to generate matches");
            return;
        }

        // Calculate the number of rounds needed
        int teamCount = tournament.ParticipatingTeams.Count;
        int totalRounds = (int)Math.Ceiling(Math.Log(teamCount, 2));
        int perfectBracketSize = (int)Math.Pow(2, totalRounds);
        int byes = perfectBracketSize - teamCount;
        int stage = GetStageNumber(tournament.CurrentStage);

        // Seed teams based on their strength
        var seededTeams = tournament.ParticipatingTeams
            .OrderByDescending(t => CalculateTeamStrength(t))
            .ToList();


        int n = seededTeams.Count;
        for (int i = 0; i < (n - byes)/2; i++)
        {
            var match = new Match
            {
                Team1 = seededTeams[i],
                Team2 = seededTeams[n - byes - 1 - i],
                Tournament = tournament,
                Year = tournament.Year,
                Week = CalculateTournamentMatchDate(tournament, stage),
                Stage = $"Round of {perfectBracketSize}",
                IsDeciderMatch = false,
                Map = GetRandomMap()
            };
            tournament.Matches.Add(match);
        }

        // Handle byes (teams that automatically advance)
        for (int i = 0; i < byes; i++)
        {
            var match = new Match
            {
                Team1 = seededTeams[i],
                Team2 = null, // No opponent - automatic win
                Tournament = tournament,
                Year = tournament.Year,
                Week = CalculateTournamentMatchDate(tournament, 1),
                Stage = $"Round of {perfectBracketSize}",
                IsCompleted = true,
                Winner = seededTeams[i],
                Map = GetRandomMap()
            };
            tournament.Matches.Add(match);
        }

        tournament.CurrentStage = $"Round of {perfectBracketSize}";
    }

    private int CalculateTournamentMatchDate(Tournament tournament, int round)
    {
        return tournament.Week + (round - 1);
    }

    private DateTime CalculateMatchDate(Tournament tournament, int round)
    {
        return new DateTime(tournament.Year, 1, 1)
            .AddDays((tournament.Week - 1) * 7)
            .AddDays(round * 2);
    }

    private void GenerateDoubleEliminationMatches(Tournament tournament)
    {
        throw new NotImplementedException("Double elimination not yet implemented");
    }

    public void GenerateSwissStageMatches(Tournament tournament)
    {
        
    }

    private bool HavePlayedBefore(Team a, Team b, Tournament tournament)
    {
        return tournament.Matches.Any(m =>
            (m.Team1 == a && m.Team2 == b) ||
            (m.Team1 == b && m.Team2 == a));
    }

    private string GetRandomMap()
    {
        var maps = new[] { "de_dust2", "de_inferno", "de_mirage", "de_train", "de_anubis", "de_ancient", "de_nuke" };
        return maps[new Random().Next(maps.Length)];
    }

    public void UpdateSwissRecords(Tournament tournament)
    {
        foreach (var team in tournament.ParticipatingTeams)
        {
            team.SwissRecord.Wins = tournament.Matches
                .Count(m => m.Winner == team && m.Stage.Contains("Swiss"));
            team.SwissRecord.Losses = tournament.Matches
                .Count(m => (m.Team1 == team || m.Team2 == team) &&
                           m.Winner != team &&
                           m.Winner != null);

            // Calculate Buchholz score (optional)
            team.SwissRecord.BuchholzScore = CalculateBuchholz(team, tournament);
        }
    }

    public float CalculateBuchholz(Team team, Tournament tournament)
    {
        // Get all opponents this team has faced in Swiss rounds
        var opponents = tournament.Matches
            .Where(m => m.Stage.Contains("Swiss") &&
                       (m.Team1 == team || m.Team2 == team) &&
                       m.Team1 != null && m.Team2 != null) // Exclude byes
            .Select(m => m.Team1 == team ? m.Team2 : m.Team1)
            .ToList();

        if (opponents.Count == 0)
            return 0f;

        // Sum all opponents' wins
        float total = 0f;
        foreach (var opponent in opponents)
        {
            total += tournament.Matches
                .Count(m => m.Winner == opponent && m.Stage.Contains("Swiss"));
        }

        // Return average to normalize for different numbers of matches
        return total / opponents.Count;
    }

    private double GetMapPerformanceFactor(Team team, string map)
    {
        if (team.MapWinRates.TryGetValue(map, out double winRate))
        {
            return 0.8 + (winRate * 0.4);
        }
        return 1.0;
    }

    private void GeneratePlayerPerformances(Match match)
    {
        if (match.Team1 == null || match.Team2 == null) return;

        foreach (var player in match.Team1.Players.Concat(match.Team2.Players))
        {
            double performanceFactor = (player.Stats.Rating / 2.0) * (random.NextDouble() * 0.4 + 0.8);

            player.MatchHistory.Add(new MatchHistory
            {
                PlayerId = player.Id,
                MatchId = match.Id,
                Kills = (int)(15 * performanceFactor),
                Deaths = (int)(20 * (1.3 - performanceFactor)),
                Assists = (int)(8 * performanceFactor),
                Rating = Math.Round(performanceFactor * 1.5, 2),
                ADR = (int)(80 * performanceFactor),
                FlashAssists = random.Next(0, 5),
                EntryKills = random.Next(0, 4),
                Map = match.Map
            });
        }
    }

    private (int Wins, int Losses) GetRecentWins(Team team, int matchCount)
    {
        var recentMatches = _gameState.CompletedMatches
            .Where(m => (m.Team1 == team || m.Team2 == team))
            .OrderByDescending(m => m.MatchDate)
            .Take(matchCount)
            .ToList();

        int wins = recentMatches.Count(m => m.Winner == team);
        return (wins, recentMatches.Count - wins);
    }

    private HeadToHeadRecord GetHeadToHeadRecord(Team team1, Team team2)
    {
        var matches = _gameState.CompletedMatches
            .Where(m => (m.Team1 == team1 && m.Team2 == team2) ||
                       (m.Team1 == team2 && m.Team2 == team1))
            .ToList();

        return new HeadToHeadRecord
        {
            Team1 = team1,
            Team2 = team2,
            Team1Wins = matches.Count(m => m.Winner == team1),
            Team2Wins = matches.Count(m => m.Winner == team2),
            Draws = matches.Count(m => m.Team1Score == m.Team2Score)
        };
    }

    private void CompleteTournament(Tournament tournament)
    {
        tournament.IsCompleted = true;
        tournament.Winner = tournament.Matches.LastOrDefault()?.Winner;

        if (tournament.Winner != null)
        {
            DistributePrizeMoney(tournament);
        }

        _gameState.ActiveTournaments.Remove(tournament);
        _gameState.CompletedTournaments.Add(tournament);

        UpdateTeamRankings();
    }

    private void ProcessGroupStageMatches(Tournament tournament)
    {
        foreach (var group in tournament.GroupStages)
        {
            foreach (var match in group.Matches.Where(m =>
                !m.IsCompleted &&
                ShouldPlayMatchThisWeek(m.Year, m.Week)))
            {
                PlayMatch(match);
            }
        }
    }

    private void ProcessPlayoffMatches(Tournament tournament)
    {
        foreach (var match in tournament.Matches.Where(m =>
            !m.IsCompleted &&
            ShouldPlayMatchThisWeek(m.Year, m.Week)))
        {
            PlayMatch(match);
        }
    }

    private void GeneratePlayoffMatches(Tournament tournament)
    {
        // Get advancing teams from all groups
        var advancingTeams = tournament.GroupStages
            .SelectMany(g => g.GetAdvancingTeams(tournament.TeamsAdvancingPerGroup))
            .ToList();

        // Clear existing playoff matches
        tournament.Matches.Clear();

        // Seed teams by their group performance
        var seededTeams = advancingTeams
            .OrderBy(t => t.GroupStageResults?.Position)
            .ThenByDescending(t => CalculateTeamStrength(t))
            .ToList();

        // Create playoff bracket
        int teamCount = seededTeams.Count;
        int bracketSize = (int)Math.Pow(2, Math.Ceiling(Math.Log(teamCount, 2)));
        int byes = bracketSize - teamCount;

        // Create first playoff round matches
        for (int i = 0; i < teamCount - byes; i += 2)
        {
            tournament.Matches.Add(new Match
            {
                Team1 = seededTeams[i],
                Team2 = seededTeams[i + 1],
                Tournament = tournament,
                MatchDate = CalculatePlayoffMatchDate(tournament, 1),
                Stage = "Playoffs - Round 1",
                IsDeciderMatch = false
            });
        }

        // Handle byes (teams that get a free pass)
        for (int i = 0; i < byes; i++)
        {
            int teamIndex = teamCount - byes + i;
            tournament.Matches.Add(new Match
            {
                Team1 = seededTeams[teamIndex],
                Tournament = tournament,
                MatchDate = CalculatePlayoffMatchDate(tournament, 1),
                Stage = "Playoffs - Round 1",
                IsCompleted = true,
                Winner = seededTeams[teamIndex]
            });
        }

        tournament.CurrentStage = "Playoffs - Round 1";
    }

    private DateTime CalculatePlayoffMatchDate(Tournament tournament, int round)
    {
        // Calculate playoff start week (group stage ends after GroupStageWeeks)
        int playoffStartWeek = tournament.Week + tournament.GroupStageWeeks;

        return new DateTime(tournament.Year, 1, 1)
            .AddDays((playoffStartWeek - 1) * 7)  // Start of playoff week
            .AddDays((round - 1) * 2);           // 2 days between matches in same round
    }

    private DateTime CalculateGroupStageMatchDate(string groupName, int teamIndex1, int teamIndex2)
    {
        int matchIndex = (teamIndex1 * 10) + teamIndex2;
        int daysOffset = matchIndex % 7;
        return new DateTime(_gameState.CurrentYear, 1, 1)
            .AddDays((_gameState.CurrentWeek - 1) * 7)
            .AddDays(daysOffset);
    }

    private bool ShouldPlayMatchThisWeek(int year, int week)
    {
        return year == _gameState.CurrentYear &&
               week == _gameState.CurrentWeek;
    }

    private void DefineTeamRegion(Team team)
    {
        int _eu = 0;
        int _am = 0;
        int _as = 0;
        foreach (var player in team.Players)
        {
            if (_EU.Contains(player.Nationality))
                _eu++;
            else if (_AM.Contains(player.Nationality))
                _am++;
            else if (_AS.Contains(player.Nationality))
                _as++;
        }
        team.Region = _as >= _eu && _as >= _am ? "AS" :
                 _am >= _eu ? "AM" : "EU";
    }

    public void MajorCreator()
    {
        int currentYear = _gameState.CurrentYear;

        // Create first major (Week 10)
        var major1 = CreateMajor(
            name: $"{GetCityName()} - {currentYear} Major",
            year: currentYear,
            week: major1Week
        );
        _gameState.ActiveTournaments.Add(major1);

        // Create second major (Week 40)
        var major2 = CreateMajor(
            name: $"{GetCityName()} - {currentYear} Major",
            year: currentYear,
            week: major2Week
        );
        _gameState.ActiveTournaments.Add(major2);

        CreateMajorRMR(major1);
        CreateMajorRMR(major2);
        this.major1 = major1;
        this.major2 = major2;
    }

    private Tournament CreateMajor(string name, int year, int week)
    {
        // Create the Major tournament
        var major = new Tournament
        {
            Id = GetNextTournamentId(),
            Name = name,
            Tier = TournamentTier.Major,
            PrizePool = 3000000,
            Year = year,
            Week = week,
            DurationWeeks = 5,
            GroupStageWeeks = 3,
            PlayoffWeeks = 1,
            ParticipatingTeams = new List<Team>(),
            Matches = new List<Match>(),
            Format = TournamentFormat.SingleElimination,
            CurrentStage = "Round of 32"
        };


        var allTeams = _gameState.AllTeams
            .OrderByDescending(t => CalculateTeamStrength(t))
            .ToList();

        // Directly invite top 16 teams (7 EU, 6 AM, 3 AS)
        var directInvites = new List<Team>();
        int euCount = 0, amCount = 0, asCount = 0;

        foreach (var team in allTeams)
        {
            if (directInvites.Count >= 16) break;

            DefineTeamRegion(team); // Ensure region is set

            if (team.Region == "EU" && euCount < 7)
            {
                directInvites.Add(team);
                euCount++;
            }
            else if (team.Region == "AM" && amCount < 6)
            {
                directInvites.Add(team);
                amCount++;
            }
            else if (team.Region == "AS" && asCount < 3)
            {
                directInvites.Add(team);
                asCount++;
            }
        }

        // Add remaining slots from RMR qualifiers (will be filled later)
        major.ParticipatingTeams.AddRange(directInvites);
        major.ParticipatingTeams.AddRange(new Team[32 - directInvites.Count]); // Placeholders

        return major;
    }

    private Tournament CreateMajorRMR(Tournament major)
    {
        var rmr = new Tournament
        {
            Id = GetNextTournamentId(),
            Name = $"{major.Name} RMR Qualifier",
            Tier = TournamentTier.RMR,
            PrizePool = 250000,
            Year = major.Year,
            Week = major.Week - 5,
            DurationWeeks = 1,
            GroupStageWeeks = 1,
            ParticipatingTeams = new List<Team>(),
            Matches = new List<Match>(),
            Format = TournamentFormat.SingleElimination,
            CurrentStage = "Round of 32",
            RelatedTournamentId = major.Id
        };

        // Take top 32 teams not already qualified
        //var rmrTeams = eligibleTeams
        //    .OrderByDescending(t => CalculateTeamStrength(t))
        //    .Take(32)
        //    .ToList();

        //rmr.ParticipatingTeams = rmrTeams;

        //// Generate matches
        //GenerateSingleEliminationMatches(rmr);

        //// Set completion handler to update major participants
        //rmr.OnCompleted = () =>
        //{
        //    var qualifiers = rmr.Matches
        //        .Where(m => m.Stage == "Grand Final")
        //        .Select(m => m.Winner)
        //        .Take(16) // Top 16 qualify
        //        .ToList();

        //    // Replace placeholder slots in major with qualifiers
        //    for (int i = 0; i < qualifiers.Count; i++)
        //    {
        //        int slotIndex = major.ParticipatingTeams.FindIndex(t => t == null);
        //        if (slotIndex >= 0)
        //        {
        //            major.ParticipatingTeams[slotIndex] = qualifiers[i];
        //        }
        //    }

        //    // Fill any remaining slots with highest ranked non-qualified teams
        //    var backupTeams = eligibleTeams.Except(qualifiers)
        //        .OrderByDescending(t => CalculateTeamStrength(t))
        //        .Take(16 - qualifiers.Count)
        //        .ToList();

        //    for (int i = 0; i < backupTeams.Count; i++)
        //    {
        //        int slotIndex = major.ParticipatingTeams.FindIndex(t => t == null);
        //        if (slotIndex >= 0)
        //        {
        //            major.ParticipatingTeams[slotIndex] = backupTeams[i];
        //        }
        //    }
        //};
        _gameState.ActiveTournaments.Add(rmr);
        return rmr;
    }

    private void PopulateRMR(Tournament rmr)
    {
        var major = _gameState.ActiveTournaments
        .FirstOrDefault(t => t.Id == rmr.RelatedTournamentId);

        var alreadyQualified = new HashSet<Team>(major.ParticipatingTeams.Where(t => t != null));

        var eligibleTeams = _gameState.AllTeams
            .Where(t => t != null && !alreadyQualified.Contains(t))
            .OrderByDescending(CalculateTeamStrength)
            .Take(32)
            .ToList();

        rmr.ParticipatingTeams = eligibleTeams;

        GenerateSingleEliminationMatches(rmr);
    }

    private void ConcludeRMR(Tournament rmr)
    {
        var major = _gameState.ActiveTournaments
            .Concat(_gameState.CompletedTournaments)
            .FirstOrDefault(t => t.Id == rmr.RelatedTournamentId);

        if (major == null) return;

        var qualifiers = rmr.Matches
            .Where(m => m.Stage == "Round of 32") // Adjust if you use different stage labels
            .Select(m => m.Winner)
            .Where(w => w != null)
            .Distinct()
            .Take(16)
            .ToList();

        // Fill empty slots in the major
        foreach (var team in qualifiers)
        {
            int slotIndex = major.ParticipatingTeams.FindIndex(t => t == null);
            if (slotIndex >= 0)
            {
                major.ParticipatingTeams[slotIndex] = team;
            }
        }

        GenerateSingleEliminationMatches(major);
    }

    public decimal ReturnBudget() => _gameState.Budget;

    public GameState GetGameState() => _gameState;
}