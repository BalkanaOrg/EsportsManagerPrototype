// GameService.cs
using EsportsManager.Models;
using System.Collections.ObjectModel;

public class GameService
{
    private GameState _gameState;
    private Random random = new Random();
    private readonly List<string> _nationalities = new()
    {
        "Germany", "Bulgaria", "UK", "France", "Spain", "Italy", "Sweden", "Denmark", "Norway", "Finland",
        "Russia", "Ukraine", "Poland", "Turkey", "Brazil", "USA", "Canada", "China", "South Korea", "Australia",
        "Netherlands", "Kazakhstan", "Serbia", "Romania", "Greece", "Albania", "Kosovo", "Mexico", "Argentina",
        "Columbia", "South Africa", "India", "Japan"
    };

    public GameService()
    {
        _gameState = new GameState();
    }

    public void InitializeNewGame(string teamName)
    {
        _gameState = new GameState();

        // Create user team with better starting budget
        decimal budget = 500000;
        _gameState.UserTeam = new Team
        {
            Name = teamName,
            Budget = budget // Increased starting budget
        };
        _gameState.AllTeams.Add(_gameState.UserTeam);
        _gameState.Budget = budget;

        // Generate AI teams with proper player counts
        GenerateAITeams(50); // 16 AI teams for competition

        // Generate quality free agents
        GenerateFreeAgents(130); // 75 free agents

        // Generate balanced initial tournaments
        GenerateInitialTournaments();

        // Ensure user team gets into some tournaments
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
                MatchHistory = new List<MatchHistory>()
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
            .Select(t => t.Id)
            .ToList();

        if (!existingIds.Any()) return 1;

        int maxId = existingIds.Max();
        int nextId = maxId + 1;

        while(existingIds.Contains(nextId))
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
        _gameState.ActiveTournaments = new List<Tournament>();
        int currentYear = _gameState.CurrentYear;

        // Major Tournament (16 teams)
        var major = CreateTournament(
            $"{GetCityName()} - {currentYear} Major",
            TournamentTier.Major,
            1000000,
            currentYear,
            10,
            3,
            32
        );
        _gameState.ActiveTournaments.Add(major);

        // Regular Tournaments (8 teams each)
        for (int i = 1; i <= 12; i++)
        {
            var tournament = CreateTournament(
                $"{GetTournamentName()} {currentYear} {i}",
                TournamentTier.A,
                250000,
                currentYear,
                2 + (i * 3),
                2,
                16
            );
            _gameState.ActiveTournaments.Add(tournament);
        }
    }

    private Tournament CreateTournament(string name, TournamentTier tier, int prizePool,
                                      int year, int week, int duration, int teamCount)
    {
        var tournament = new Tournament
        {
            Id = GetNextTournamentId(),
            Name = name,
            Tier = tier,
            PrizePool = prizePool,
            Year = year,
            Week = week,
            DurationWeeks = duration,
            ParticipatingTeams = new List<Team>(),
            Matches = new List<Match>(),
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
        player.CurrentTeamId = team.Id;
        team.Budget -= player.Buyout;
    }

    public void ReleasePlayer(Player player)
    {
        var team = _gameState.AllTeams.FirstOrDefault(t => t.Id == player.CurrentTeamId);
        if (team != null)
        {
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

    public void ProcessNextWeek()
    {
        // Process all tournaments that are active this week
        var currentTournaments = _gameState.ActiveTournaments
        .Where(t => t.Year == _gameState.CurrentYear &&
                    t.Week <= _gameState.CurrentWeek &&
                    (t.Week + t.DurationWeeks) > _gameState.CurrentWeek)
        .ToList(); // <- to avoid modifying during loop

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
        // For simplicity, we'll just generate matches for this week
        // In a real implementation, you'd have a more sophisticated tournament structure

        if (tournament.Matches.Count == 0)
        {
            // Generate initial matches for the tournament
            GenerateInitialMatches(tournament);
        }
        else
        {
            // Process next round of matches
            GenerateNextRoundMatches(tournament);
        }

        // Play matches for this week
        foreach (var match in tournament.Matches
            .Where(m => !m.IsCompleted &&
                       m.MatchDate.Year == _gameState.CurrentYear &&
                       m.MatchDate.DayOfYear / 7 + 1 == _gameState.CurrentWeek))
        {
            PlayMatch(match);
        }

        // Check if tournament is completed
        if (tournament.Matches.All(m => m.IsCompleted) && !tournament.IsCompleted)
        {
            tournament.IsCompleted = true;
            tournament.Winner = DetermineTournamentWinner(tournament);
            _gameState.ActiveTournaments.Remove(tournament);
            _gameState.CompletedTournaments.Add(tournament);

            // Distribute prize money
            DistributePrizeMoney(tournament);
        }
    }

    private void GenerateInitialMatches(Tournament tournament)
    {
        // Simple implementation - just pair teams randomly
        var teams = tournament.ParticipatingTeams.OrderBy(t => Guid.NewGuid()).ToList();

        for (int i = 0; i < teams.Count; i += 2)
        {
            if (i + 1 < teams.Count)
            {
                tournament.Matches.Add(new Match
                {
                    Team1 = teams[i],
                    Team2 = teams[i + 1],
                    Tournament = tournament,
                    MatchDate = new DateTime(_gameState.CurrentYear, 1, 1).AddDays((_gameState.CurrentWeek - 1) * 7)
                });
            }
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
        double team1Strength = CalculateTeamStrength(match.Team1);
        double team2Strength = CalculateTeamStrength(match.Team2);

        // Add realistic randomness (0.7 to 1.3 multiplier)
        team1Strength *= (random.NextDouble() * 0.6) + 0.7;
        team2Strength *= (random.NextDouble() * 0.6) + 0.7;

        // Best of 1 (MR15)
        int team1Rounds = (int)(16 * (team1Strength / (team1Strength + team2Strength)));
        int team2Rounds = 16 - team1Rounds;

        // Ensure at least 1 round difference
        if (team1Rounds == team2Rounds)
        {
            if (random.Next(2) == 0) team1Rounds++;
            else team2Rounds++;
        }

        match.Team1Score = team1Rounds;
        match.Team2Score = team2Rounds;
        match.IsCompleted = true;

        // Enhanced player performance generation
        GenerateRealisticPlayerPerformances(match);

        _gameState.CompletedMatches.Add(match);
    }

    private double CalculateTeamStrength(Team team)
    {
        if (team.Players.Count == 0) return 0;

        double totalStrength = 0;
        int count = 0;

        foreach (var player in team.Players)
        {
            double playerStrength = player.Stats.Rating;

            // Boost for team synergy
            if (team.Players.Count >= 3)
                playerStrength *= 1.1;

            totalStrength += playerStrength;
            count++;
        }

        return totalStrength / count;
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

    private void UpdateTeamRankings()
    {
        // Get all completed matches across all tournaments
        var allMatches = _gameState.ActiveTournaments
            .Concat(_gameState.CompletedTournaments)
            .SelectMany(t => t.Matches)
            .Where(m => m.IsCompleted)
            .ToList();

        // Calculate team rankings
        var teamsWithScores = _gameState.AllTeams.Select(team =>
        {
            // Get all matches where this team participated
            var teamMatches = allMatches
                .Where(m => m.Team1 == team || m.Team2 == team)
                .ToList();

            // Calculate points (3 for win, 1 for loss)
            int points = teamMatches.Sum(m =>
                m.Winner == team ? 3 : 1);

            return new
            {
                Team = team,
                Points = points,
                Wins = teamMatches.Count(m => m.Winner == team),
                Losses = teamMatches.Count(m => m.Winner != team)
            };
        })
        .OrderByDescending(t => t.Points)
        .ThenByDescending(t => t.Wins)
        .ToList();

        // Assign rankings
        for (int i = 0; i < teamsWithScores.Count; i++)
        {
            teamsWithScores[i].Team.WorldRanking = i + 1;
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

    public Tournament GetTournamentById(int tournamentId)
    {
        // Search in active tournaments first
        var tournament = _gameState.ActiveTournaments
            .Concat(_gameState.CompletedTournaments)
            .FirstOrDefault(t => t.Id == tournamentId);

        return tournament;
    }

    public decimal ReturnBudget() => _gameState.Budget;

    public GameState GetGameState() => _gameState;
}