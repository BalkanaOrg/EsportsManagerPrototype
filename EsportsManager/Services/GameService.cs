// GameService.cs
using EsportsManager.Models;
using System.Collections.ObjectModel;
using System.Security.Principal;

public class GameService
{
    private GameState _gameState;
    private Random random = new Random();

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
            Name = teamName,
            Budget = budget,
            Color = color
        };
        _gameState.AllTeams.Add(_gameState.UserTeam);
        _gameState.Budget = budget;

        GenerateAITeams(200);

        GenerateFreeAgents(130);

        GenerateInitialTournaments();

        AssignInitialTournamentParticipation();
    }

    private void GenerateAITeams(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var aiTeam = new Team
            {
                Id = _gameState.AllTeams.Count + 1, // Ensure unique ID
                Name = GenerateTeamName(),
                Budget = 300000 + random.Next(0, 400000),
                //Players = new List<Player>(),
                //Bench = new List<Player>(),
                Players = new ObservableCollection<Player>(),
                Bench = new ObservableCollection<Player>(),
                MatchHistory = new List<MatchHistory>(),
                Color = _teamColors[random.Next(_teamColors.Count)],
            };

            // Generate 5 main + 2 bench players
            for (int j = 0; j < 7; j++)
            {
                var player = GeneratePlayerForTeam(aiTeam.Id, j < 3); // First 3 are stronger
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
            player.Stats.Rating = 1.3;//PlayerGeneratorService.CalculateOverallRating(player.Stats);
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

    private void BoostPlayerStats(Player player)
    {
        // Make star players better
        //player.Stats.Aim = Math.Min(100, player.Stats.Aim + 10);
        //player.Stats.Rating = PlayerGeneratorService.CalculateOverallRating(player.Stats);
        //player.MarketValue = PlayerGeneratorService.CalculateMarketValue(player.Stats, player.Age);
        //player.Salary = PlayerGeneratorService.CalculateSalary(player.Stats, player.Age);
    }

    private void GenerateFreeAgents(int count)
    {
        _gameState.FreeAgents = new List<Player>(); // Clear existing

        for (int i = 0; i < count; i++)
        {
            string nationality = _nationalities[random.Next(_nationalities.Count)];
            var player = PlayerGeneratorService.GeneratePlayer(nationality);
            player.Id = GetNextPlayerId();
            player.CurrentTeamId = 0; // Important: Mark as free agent

            // Ensure some top-tier free agents
            if (i < 10) // First 10 are better
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
        _gameState.UpcomingTournaments = new List<Tournament>();
        int currentYear = _gameState.CurrentYear;

        var major = CreateTournament(
            name: $"{GetCityName()} - {currentYear} Major",
            tier: TournamentTier.Major,
            prizePool: 3000000,
            year: currentYear,
            startWeek: 15,
            durationWeeks: 3,
            teamCount: 32,
            format: TournamentFormat.SingleElimination
        );
        major.CurrentStage = "Round of 32";
        _gameState.UpcomingTournaments.Add(major);


        //Major RMR
        var rmr = CreateTournament(
            name: $"{major.Name} - {currentYear} RMR",
            tier: TournamentTier.RMR,
            prizePool: 1000000,
            year: currentYear,
            startWeek: major.Week-5,
            durationWeeks: 2,
            teamCount: 32,
            format: TournamentFormat.SingleElimination
        );
        rmr.CurrentStage = "Round of 32";
        _gameState.UpcomingTournaments.Add(rmr);

        //Random S-tier tournaments
        for (int i = 1; i <= 12; i++)
        {
            var tournament = CreateTournament(
                name: $"{GetTournamentName()} {currentYear} {i}",
                tier: TournamentTier.S,
                prizePool: 250000,
                year: currentYear,
                startWeek: 2 + (i * 3),
                durationWeeks: 2,
                teamCount: 16,
                format: TournamentFormat.RoundRobin
            );
            tournament.CurrentStage = "Group Stage";
            tournament.TeamsPerGroup = 4; // 4 groups of 4 teams
            tournament.TeamsAdvancingPerGroup = 2; // Top 2 from each group advance
            _gameState.UpcomingTournaments.Add(tournament);
        }

        for(int i = 1; i<=2; i++)
        {
            var tournament = CreateTournament(
                name: $"ESL Pro League {currentYear} {i}",
                tier: TournamentTier.A,
                prizePool: 1000000,
                year: currentYear,
                startWeek: 20 * i,
                durationWeeks: 4,
                teamCount: 32,
                format: TournamentFormat.RoundRobin
                );
            tournament.CurrentStage = "Group Stage";    
            tournament.TeamsPerGroup = 8;
            tournament.TeamsAdvancingPerGroup = 2;
            _gameState.UpcomingTournaments.Add(tournament);
        }
    }

    // Updated CreateTournament method signature
    private Tournament CreateTournament(
        string name,
        TournamentTier tier,
        int prizePool,
        int year,
        int startWeek,
        int durationWeeks,
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
            ParticipatingTeams = new List<Team>(),
            Matches = new List<Match>(),
            GroupStages = new List<GroupStage>(),
            Format = format,
            IsCompleted = false
        };

        // Select top teams plus user team
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

        // Generate appropriate matches based on format
        switch (format)
        {
            case TournamentFormat.SingleElimination:
                GenerateSingleEliminationMatches(tournament);
                break;
            case TournamentFormat.RoundRobin:
                GenerateRoundRobinMatches(tournament);
                break;
                // Add cases for other formats as needed
        }

        return tournament;
    }

    
    private void AssignInitialTournamentParticipation()
    {
        foreach (var tournament in _gameState.ActiveTournaments)
        {
            // Select top teams plus user team
            var participants = _gameState.AllTeams
                .OrderByDescending(t => CalculateTeamStrength(t))
                .Take(tournament.Tier == TournamentTier.Major ? 32 : 16)
                .ToList();

            // Ensure user team gets into some tournaments
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
            _gameState.UpcomingTournaments.Add(new Tournament
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
            _gameState.UpcomingTournaments.Add(new Tournament
            {
                Id = GetNextTournamentId(),
                Name = $"{GetTournamentName()} {year}",
                Tier = TournamentTier.A,
                PrizePool = 250000,
                Year = year,
                Week = random.Next(5, 45),
                DurationWeeks = 1
            });
        }

        // Add lower tier tournaments similarly...
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

        // Remove from free agents or previous team
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

        // Add to new team
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
        // Fetch team by ID from your data source, such as an API or database
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
    public void MoveToActive(Tournament t)
    {
        var tournament = _gameState.UpcomingTournaments.FirstOrDefault(t=>t.Id == t.Id);
        if(tournament!=null && !(_gameState.ActiveTournaments.Contains(tournament)))
        {
            _gameState.ActiveTournaments.Add(tournament);
            _gameState.UpcomingTournaments.Remove(tournament);
        }
    }

    public void ProcessNextWeek()
    {
        _gameState.ActiveTournaments = _gameState.UpcomingTournaments
            .Where(t => t.Year == _gameState.CurrentYear &&
                        t.Week <= _gameState.CurrentWeek &&
                        (t.Week + t.DurationWeeks) > _gameState.CurrentWeek)
            .ToList();
        _gameState.UpcomingTournaments.RemoveAll(t => _gameState.ActiveTournaments.Contains(t));
        // Process all tournaments that are active this week
        var currentTournaments = _gameState.ActiveTournaments
            .Where(t => t.Year == _gameState.CurrentYear &&
                        t.Week <= _gameState.CurrentWeek &&
                        (t.Week + t.DurationWeeks) > _gameState.CurrentWeek)
            .ToList();

        foreach (var tournament in currentTournaments)
        {
            ProcessTournamentWeek(tournament);
        }

        // Advance week
        _gameState.CurrentWeek++;
        if (_gameState.CurrentWeek > 52)
        {
            _gameState.CurrentWeek = 1;
            _gameState.CurrentYear++;

            // Generate new tournaments for the new year
            GenerateTournamentsForYear(_gameState.CurrentYear);
        }
        _gameState.Budget -= _gameState.WeeklyExpense;
        // Update team rankings
        UpdateTeamRankings();
    }

    private void ProcessTournamentWeek(Tournament tournament)
    {
        if (tournament.IsCompleted) return;

        // Process group stage matches first if they exist
        if (tournament.HasGroupStage)
        {
            ProcessGroupStageMatches(tournament);
            ProcessGroupStage(tournament);
        }

        // Then process playoff matches
        ProcessPlayoffMatches(tournament);

        // Check if tournament is complete
        if (tournament.Matches.Any() &&
            tournament.Matches.All(m => m.IsCompleted) &&
            !tournament.IsCompleted)
        {
            CompleteTournament(tournament);
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

        // Determine group configuration
        int teamCount = tournament.ParticipatingTeams.Count;
        int groupSize = teamCount switch
        {
            < 16 => 4,  // 4 teams per group for small tournaments
            < 32 => 8,  // 8 teams per group for medium tournaments
            _ => 16    // 16 teams per group for large tournaments
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

        // Calculate momentum (1.0 = neutral)
        double momentum = 1.0;
        momentum += (team1Recent.Wins - team2Recent.Wins) * 0.05;
        momentum += (headToHead.Team1Wins - headToHead.Team2Wins) * 0.03;

        return Math.Clamp(momentum, 0.8, 1.2);
    }


    private void AdvanceTournamentStage(Tournament tournament)
    {
        switch (tournament.Format)
        {
            case TournamentFormat.SingleElimination:
                AdvanceSingleElimination(tournament);
                break;

                // Implement other formats similarly
        }
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
            // Extract the number from the stage name
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

        return 0; // fallback
    }

    private int GetStageNumber(string stage)
    {
        return stage switch
        {
            "Round of 64" => 1,
            "Round of 32" => 2,
            "Round of 16" => 3,
            "Quarterfinals" => 4,
            "Semifinals" => 5,
            "Grand Final" => 6,
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

            case TournamentFormat.Swiss:
                GenerateSwissMatches(tournament);
                break;
        }
    }
    private void GenerateNextRoundMatches(Tournament tournament)
    {
        // Simple implementation - just pair winners from previous matches
        var winners = tournament.Matches
            .Where(m => m.IsCompleted)
            .Select(m => m.Winner)
            .Distinct()
            .OrderBy(t => Guid.NewGuid())
            .ToList();

        tournament.Matches.Clear(); // Clear old matches

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
                // Handle odd number of teams (give a bye)
                winners[i].Budget += tournament.PrizePool * 0.1m; // Small prize for bye
            }
        }
    }

    private void PlayMatch(Match match)
    {
        if (match.Team2 == null) return; // Bye match

        double team1Strength = CalculateTeamStrength(match.Team1);
        double team2Strength = CalculateTeamStrength(match.Team2);

        // Apply map-specific adjustments (you could add map property to Match)
        team1Strength *= GetMapPerformanceFactor(match.Team1, "de_dust2");
        team2Strength *= GetMapPerformanceFactor(match.Team2, "de_dust2");

        // Add realistic randomness and momentum
        double momentumFactor = CalculateMomentumFactor(match.Team1, match.Team2);
        team1Strength *= momentumFactor;
        team2Strength *= (2 - momentumFactor); // Inverse for other team

        // Add clutch factor for close matches
        double clutchFactor = 1 + (random.NextDouble() * 0.2 - 0.1); // ±10%

        // Calculate round scores (MR15)
        int team1Rounds = (int)(16 * (team1Strength / (team1Strength + team2Strength)) * clutchFactor);
        int team2Rounds = 16 - team1Rounds;

        // Ensure minimum 1 round difference
        if (team1Rounds == team2Rounds)
        {
            if (random.Next(2) == 0) team1Rounds++;
            else team2Rounds++;
        }

        match.Team1Score = Math.Min(16, team1Rounds);
        match.Team2Score = Math.Min(16, team2Rounds);
        match.IsCompleted = true;
        match.Winner = team1Rounds > team2Rounds ? match.Team1 : match.Team2;

        GeneratePlayerPerformances(match);
        UpdateTeamMomentum(match);
    }

    private void UpdateTeamMomentum(Match match)
    {
        // Winner gains momentum, loser loses some
        match.Winner.Momentum = Math.Min(1.2, match.Winner.Momentum * 1.05);
        Team loser = match.Winner == match.Team1 ? match.Team2 : match.Team1;
        loser.Momentum = Math.Max(0.8, loser.Momentum * 0.95);
    }

    //private double CalculateTeamStrength(Team team)
    //{
    //    if (team.Players.Count == 0) return 0;

    //    double totalStrength = 0;
    //    int count = 0;

    //    foreach (var player in team.Players)
    //    {
    //        double playerStrength = player.Stats.Rating;

    //        // Boost for team synergy
    //        if (team.Players.Count >= 3)
    //            playerStrength *= 1.1;

    //        totalStrength += playerStrength;
    //        count++;
    //    }

    //    return totalStrength / count;
    //}

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

    //public void UpdateTeamRankings()
    //{
    //    // Get all completed matches across all tournaments
    //    var allMatches = _gameState.ActiveTournaments
    //        .Concat(_gameState.CompletedTournaments)
    //        .SelectMany(t => t.Matches)
    //        .Where(m => m.IsCompleted)
    //        .ToList();

    //    // Calculate team rankings
    //    var teamsWithScores = _gameState.AllTeams.Select(team =>
    //    {
    //        // Get all matches where this team participated
    //        var teamMatches = allMatches
    //            .Where(m => m.Team1 == team || m.Team2 == team)
    //            .ToList();

    //        // Calculate points (3 for win, 1 for loss)
    //        int points = teamMatches.Sum(m =>
    //            m.Winner == team ? 3 : 1);

    //        return new
    //        {
    //            Team = team,
    //            Points = points,
    //            Wins = teamMatches.Count(m => m.Winner == team),
    //            Losses = teamMatches.Count(m => m.Winner != team)
    //        };
    //    })
    //    .OrderByDescending(t => t.Points)
    //    .ThenByDescending(t => t.Wins)
    //    .ToList();

    //    // Assign rankings
    //    for (int i = 0; i < teamsWithScores.Count; i++)
    //    {
    //        teamsWithScores[i].Team.WorldRanking = i + 1;
    //    }
    //}

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
            teamScores[i].Team.TeamScore = teamScores[i].TotalScore; // Store for reference
        }
    }

    private List<Match> GetAllCompletedMatches()
    {
        return _gameState.ActiveTournaments
            .Concat(_gameState.CompletedTournaments)
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

        // Apply penalties for missing roles
        if (!hasRifler) compositionScore *= 0.85;
        if (!hasAWPer) compositionScore *= 0.80;
        if (!hasIGL) compositionScore *= 0.70; // IGL is very important
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

        // Synergy bonuses
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

        if (!teamMatches.Any()) return 1.0; // Neutral if no matches

        int wins = 0;
        int matches = 0;
        double totalPerformance = 0;

        foreach (var match in teamMatches)
        {
            bool isTeam1 = match.Team1 == team;
            bool won = isTeam1 ? match.Team1Score > match.Team2Score : match.Team2Score > match.Team1Score;

            if (won) wins++;
            matches++;

            // Calculate performance ratio (rounds won / total rounds)
            double performance = isTeam1 ?
                (double)match.Team1Score / (match.Team1Score + match.Team2Score) :
                (double)match.Team2Score / (match.Team1Score + match.Team2Score);

            totalPerformance += performance;
        }

        double winRate = (double)wins / matches;
        double avgPerformance = totalPerformance / matches;

        // Combine win rate and performance (60/40 weighting)
        return (winRate * 0.6) + (avgPerformance * 0.4);
    }

    private void GenerateSingleEliminationMatches(Tournament tournament)
    {
        GenerateSingleEliminationMatches(tournament, tournament.ParticipatingTeams);
    }

    private void GenerateSingleEliminationMatches(Tournament tournament, List<Team> teams)
    {
        // Check if we need byes (when team count isn't power of 2)
        int teamsNeeded = (int)Math.Pow(2, Math.Ceiling(Math.Log(teams.Count, 2)));
        int byes = teamsNeeded - teams.Count;

        // Create first round matches
        for (int i = 0; i < teams.Count - byes; i += 2)
        {
            tournament.Matches.Add(new Match
            {
                Team1 = teams[i],
                Team2 = teams[i + 1],
                Tournament = tournament,
                MatchDate = CalculateMatchDate(tournament, 0),
                Stage = "Round of " + teamsNeeded,
                IsDeciderMatch = false
            });
        }

        // Teams with byes automatically advance
        for (int i = 0; i < byes; i++)
        {
            int teamIndex = teams.Count - byes + i;
            tournament.Matches.Add(new Match
            {
                Team1 = teams[teamIndex],
                Team2 = null, // Indicates a bye
                Tournament = tournament,
                MatchDate = CalculateMatchDate(tournament, 0),
                Stage = "Round of " + teamsNeeded,
                IsDeciderMatch = false,
                IsCompleted = true,
                Winner = teams[teamIndex]
            });
        }

        tournament.CurrentStage = "Round of " + teamsNeeded;
    }

    private DateTime CalculateMatchDate(Tournament tournament, int round)
    {
        return new DateTime(tournament.Year, 1, 1)
            .AddDays((tournament.Week - 1) * 7)
            .AddDays(round * 2); // Matches every 2 days during tournament
    }

    private void GenerateDoubleEliminationMatches(Tournament tournament)
    {
        // Implement double elimination logic
        // Similar to single elimination but with loser's bracket
        throw new NotImplementedException("Double elimination not yet implemented");
    }

    private void GenerateSwissMatches(Tournament tournament)
    {
        // Implement Swiss system pairing
        throw new NotImplementedException("Swiss system not yet implemented");
    }

    private double GetMapPerformanceFactor(Team team, string map)
    {
        if (team.MapWinRates.TryGetValue(map, out double winRate))
        {
            return 0.8 + (winRate * 0.4); // Convert to 0.8-1.2 factor
        }
        return 1.0; // Neutral if no data
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

        // Distribute prize money
        if (tournament.Winner != null)
        {
            DistributePrizeMoney(tournament);
        }

        _gameState.ActiveTournaments.Remove(tournament);
        _gameState.CompletedTournaments.Add(tournament);

        // Update team rankings
        UpdateTeamRankings();
    }

    private void ProcessGroupStageMatches(Tournament tournament)
    {
        foreach (var group in tournament.GroupStages)
        {
            foreach (var match in group.Matches.Where(m =>
                !m.IsCompleted &&
                ShouldPlayMatchThisWeek(m.MatchDate)))
            {
                PlayMatch(match);
            }
        }
    }

    private void ProcessPlayoffMatches(Tournament tournament)
    {
        foreach (var match in tournament.Matches.Where(m =>
            !m.IsCompleted &&
            ShouldPlayMatchThisWeek(m.MatchDate)))
        {
            PlayMatch(match);
        }
    }

    private void GeneratePlayoffMatches(Tournament tournament)
    {
        var advancingTeams = tournament.GroupStages
            .SelectMany(g => g.GetAdvancingTeams(tournament.TeamsAdvancingPerGroup))
            .OrderBy(t => Guid.NewGuid()) // Randomize for bracket
            .ToList();

        GenerateSingleEliminationMatches(tournament, advancingTeams);
    }

    private DateTime CalculateGroupStageMatchDate(string groupName, int teamIndex1, int teamIndex2)
    {
        // Spread matches evenly across group stage duration
        int matchIndex = (teamIndex1 * 10) + teamIndex2; // Simple hash
        int daysOffset = matchIndex % 7; // Spread over a week
        return new DateTime(_gameState.CurrentYear, 1, 1)
            .AddDays((_gameState.CurrentWeek - 1) * 7)
            .AddDays(daysOffset);
    }

    private bool ShouldPlayMatchThisWeek(DateTime matchDate)
    {
        return matchDate.Year == _gameState.CurrentYear &&
               matchDate.DayOfYear / 7 + 1 == _gameState.CurrentWeek;
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

    public decimal ReturnBudget() => _gameState.Budget;

    public GameState GetGameState() => _gameState;
}