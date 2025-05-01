//using EsportsManager.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace EsportsManager.Services
//{
//    public class TournamentService
//    {
//        private readonly GameState _gameState;
//        private Random random = new Random();
//        public TournamentService(GameState gameState)
//        {
//            _gameState = gameState;
//        }

//        public int GetNextTournamentId()
//        {
//            var existingIds = _gameState.ActiveTournaments
//                .Concat(_gameState.CompletedTournaments)
//                .Concat(_gameState.UpcomingTournaments)
//                .Select(t => t.Id)
//                .ToList();

//            if (!existingIds.Any()) return 1;

//            int maxId = existingIds.Max();
//            int nextId = maxId + 1;

//            while (existingIds.Contains(nextId))
//            {
//                nextId++;
//            }

//            Console.WriteLine($"Generated new tournament ID: {nextId}");
//            return nextId;
//        }

//        private void GenerateInitialTournaments()
//        {
//            //Major
//            _gameState.UpcomingTournaments = new List<Tournament>();
//            int currentYear = _gameState.CurrentYear;

//            var major = CreateTournament(
//                name: $"{GetCityName()} - {currentYear} Major",
//                tier: TournamentTier.Major,
//                prizePool: 3000000,
//                year: currentYear,
//                startWeek: 15,
//                durationWeeks: 3,
//                teamCount: 32,
//                format: TournamentFormat.SingleElimination
//            );
//            major.CurrentStage = "Round of 32";
//            _gameState.UpcomingTournaments.Add(major);


//            //Major RMR
//            var rmr = CreateTournament(
//                name: $"{major.Name} - {currentYear} RMR",
//                tier: TournamentTier.RMR,
//                prizePool: 1000000,
//                year: currentYear,
//                startWeek: major.Week - 5,
//                durationWeeks: 2,
//                teamCount: 32,
//                format: TournamentFormat.SingleElimination
//            );
//            rmr.CurrentStage = "Round of 32";
//            _gameState.UpcomingTournaments.Add(rmr);

//            //Random S-tier tournaments
//            for (int i = 1; i <= 12; i++)
//            {
//                var tournament = CreateTournament(
//                    name: $"{GetTournamentName()} {currentYear} {i}",
//                    tier: TournamentTier.S,
//                    prizePool: 250000,
//                    year: currentYear,
//                    startWeek: 2 + (i * 3),
//                    durationWeeks: 2,
//                    teamCount: 16,
//                    format: TournamentFormat.RoundRobin
//                );
//                tournament.CurrentStage = "Group Stage";
//                tournament.TeamsPerGroup = 4; // 4 groups of 4 teams
//                tournament.TeamsAdvancingPerGroup = 2; // Top 2 from each group advance
//                _gameState.UpcomingTournaments.Add(tournament);
//            }

//            for (int i = 1; i <= 2; i++)
//            {
//                var tournament = CreateTournament(
//                    name: $"ESL Pro League {currentYear} {i}",
//                    tier: TournamentTier.A,
//                    prizePool: 1000000,
//                    year: currentYear,
//                    startWeek: 20 * i,
//                    durationWeeks: 4,
//                    teamCount: 32,
//                    format: TournamentFormat.RoundRobin
//                    );
//                tournament.CurrentStage = "Group Stage";
//                tournament.TeamsPerGroup = 8;
//                tournament.TeamsAdvancingPerGroup = 2;
//                _gameState.UpcomingTournaments.Add(tournament);
//            }
//        }

//        // Updated CreateTournament method signature
//        private Tournament CreateTournament(
//            string name,
//            TournamentTier tier,
//            int prizePool,
//            int year,
//            int startWeek,
//            int durationWeeks,
//            int teamCount,
//            TournamentFormat format)
//        {
//            var tournament = new Tournament
//            {
//                Id = GetNextTournamentId(),
//                Name = name,
//                Tier = tier,
//                PrizePool = prizePool,
//                Year = year,
//                Week = startWeek,
//                DurationWeeks = durationWeeks,
//                ParticipatingTeams = new List<Team>(),
//                Matches = new List<Match>(),
//                GroupStages = new List<GroupStage>(),
//                Format = format,
//                IsCompleted = false
//            };

//            // Select top teams plus user team
//            var participants = _gameState.AllTeams
//                .OrderByDescending(t => CalculateTeamStrength(t))
//                .Take(teamCount)
//                .ToList();

//            if (!participants.Contains(_gameState.UserTeam))
//            {
//                participants.RemoveAt(participants.Count - 1);
//                participants.Add(_gameState.UserTeam);
//            }

//            tournament.ParticipatingTeams = participants;

//            // Generate appropriate matches based on format
//            switch (format)
//            {
//                case TournamentFormat.SingleElimination:
//                    GenerateSingleEliminationMatches(tournament);
//                    break;
//                case TournamentFormat.RoundRobin:
//                    GenerateRoundRobinMatches(tournament);
//                    break;
//                    // Add cases for other formats as needed
//            }

//            return tournament;
//        }

//        private void AssignInitialTournamentParticipation()
//        {
//            foreach (var tournament in _gameState.ActiveTournaments)
//            {
//                // Select top teams plus user team
//                var participants = _gameState.AllTeams
//                    .OrderByDescending(t => CalculateTeamStrength(t))
//                    .Take(tournament.Tier == TournamentTier.Major ? 32 : 16)
//                    .ToList();

//                // Ensure user team gets into some tournaments
//                if (!participants.Contains(_gameState.UserTeam))
//                {
//                    participants.RemoveAt(participants.Count - 1);
//                    participants.Add(_gameState.UserTeam);
//                }

//                tournament.ParticipatingTeams = participants;
//            }
//        }

//        private string GenerateTeamName()
//        {
//            string[] cities = { "Sofia", "Belgrade", "Bucharest", "Istanbul", "Berlin", "Frankfurt", "Budapest", "Paris", "London", "Edinbourgh", "Liverpool", "Manchester", "Madrid", "Lisbon", "Moscow", "Kiyv", "Stockholm", "Oslo", "Washington", "Los Angeles", "Miami", "Beijing", "Shanghai", "Warsaw", "Ontario", "Austin", "Dallas", "Ankara", "Tokyo", "Astana", "Copenhagen", "Athens", "New Delhi", "Mumbai" };
//            string[] prefixes = { "Team", "Esports", "Gaming", "Pro", "Elite", "Prime", "Alpha", "Omega", "Royal", "Noble", "Bulgarian", "German", "English", "French", "British", "Spanish", "Turkish" };
//            string[] suffixes = { "Kings", "Legion", "Squad", "Force", "Nation", "Empire", "Dynasty", "Club", "Collective", "Assembly", "Shock", "Defiant", "Eternal", "Hunters", "Charge", "Spark", "Infernal", "Justice", "Spitfire" };
//            string[] adjectives = { "Red", "Blue", "Black", "White", "Golden", "Silver", "Dark", "Light", "Mighty", "Fierce", "Unstoppable", "Shocking", "Liquid", "Solid", "Purple", "Pink", "Violet", "Proud", "Cloudy", "Sunny", "Platinum" };
//            string[] nouns = { "Wolves", "Eagles", "Dragons", "Lions", "Tigers", "Sharks", "Falcons", "Hawks", "Bears", "Rhinos", "Dolphins", "Swords", "Shields", "Warriors", "Kings", "Might", "Valiance", "Cowboys", "Fuel", "Outlaws", "Giants", "Elfs", "Guns", "Samurais", "Guards" };

//            int pattern = random.Next(4);
//            return pattern switch
//            {
//                0 => $"{prefixes[random.Next(prefixes.Length)]} {suffixes[random.Next(suffixes.Length)]}",
//                1 => $"{adjectives[random.Next(adjectives.Length)]} {nouns[random.Next(nouns.Length)]}",
//                2 => $"{prefixes[random.Next(prefixes.Length)]} {nouns[random.Next(nouns.Length)]}",
//                3 => $"{cities[random.Next(cities.Length)]} {suffixes[random.Next(suffixes.Length)]}",
//                4 => $"{cities[random.Next(cities.Length)]} {nouns[random.Next(nouns.Length)]}",
//                _ => $"{adjectives[random.Next(adjectives.Length)]} {suffixes[random.Next(suffixes.Length)]}"
//            };
//        }

//        private void GenerateTournamentsForYear(int year)
//        {
//            // Majors (2 per year)
//            for (int i = 0; i < 2; i++)
//            {
//                _gameState.UpcomingTournaments.Add(new Tournament
//                {
//                    Id = GetNextTournamentId(),
//                    Name = $"{GetCityName()} - {year} Major",
//                    Tier = TournamentTier.Major,
//                    PrizePool = 1000000,
//                    Year = year,
//                    Week = random.Next(10, 40),
//                    DurationWeeks = 2
//                });
//            }

//            // Tier A tournaments (6 per year)
//            for (int i = 0; i < 6; i++)
//            {
//                _gameState.UpcomingTournaments.Add(new Tournament
//                {
//                    Id = GetNextTournamentId(),
//                    Name = $"{GetTournamentName()} {year}",
//                    Tier = TournamentTier.A,
//                    PrizePool = 250000,
//                    Year = year,
//                    Week = random.Next(5, 45),
//                    DurationWeeks = 1
//                });
//            }

//            // Add lower tier tournaments similarly...
//        }

//        private string GetCityName()
//        {
//            string[] cities = { "Berlin", "Paris", "London", "Stockholm", "Los Angeles", "Shanghai", "Seoul", "Moscow", "Sydney", "Rio", "Belgrade", "Budapest", "Sofia", "Beijing", "Copenhagen", "Lisbon" };
//            return cities[random.Next(cities.Length)];
//        }

//        private string GetTournamentName()
//        {
//            string[] names = { "HackedDreams", "Electronic League", "IEMbg", "BOOM", "EPIC", "Esports World Cup", "Academy", "FACEUP", "StarLadder", "PGL", "Balkana" };
//            return names[random.Next(names.Length)];
//        }

//        private void ProcessGroupStage(Tournament tournament)
//        {
//            if (!tournament.HasGroupStage) return;

//            foreach (var group in tournament.GroupStages)
//            {
//                group.CalculateStandings();
//            }

//            bool allGroupsComplete = tournament.GroupStages
//                .All(g => g.Matches.All(m => m.IsCompleted));

//            if (allGroupsComplete && !tournament.Matches.Any())
//            {
//                GeneratePlayoffMatches(tournament);
//            }
//        }

//        private void GenerateRoundRobinMatches(Tournament tournament)
//        {
//            tournament.Matches.Clear();
//            tournament.GroupStages.Clear();

//            // Determine group configuration
//            int teamCount = tournament.ParticipatingTeams.Count;
//            int groupSize = teamCount switch
//            {
//                < 16 => 4,  // 4 teams per group for small tournaments
//                < 32 => 8,  // 8 teams per group for medium tournaments
//                _ => 16    // 16 teams per group for large tournaments
//            };

//            int groupCount = (int)Math.Ceiling((double)teamCount / groupSize);

//            // Create groups
//            var shuffledTeams = tournament.ParticipatingTeams
//                .OrderBy(t => Guid.NewGuid())
//                .ToList();

//            for (int i = 0; i < groupCount; i++)
//            {
//                var group = new GroupStage
//                {
//                    Name = $"Group {(char)('A' + i)}",
//                    Teams = shuffledTeams.Skip(i * groupSize).Take(groupSize).ToList()
//                };

//                GenerateRoundRobinMatchesForGroup(group);
//                tournament.GroupStages.Add(group);
//            }

//            tournament.TeamsPerGroup = groupSize;
//            tournament.TeamsAdvancingPerGroup = Math.Max(2, groupSize / 2);
//        }

//        private void GenerateRoundRobinMatchesForGroup(GroupStage group)
//        {
//            for (int i = 0; i < group.Teams.Count; i++)
//            {
//                for (int j = i + 1; j < group.Teams.Count; j++)
//                {
//                    group.Matches.Add(new Match
//                    {
//                        Team1 = group.Teams[i],
//                        Team2 = group.Teams[j],
//                        Stage = group.Name,
//                        MatchDate = CalculateGroupStageMatchDate(group.Name, i, j),
//                        IsDeciderMatch = false
//                    });
//                }
//            }
//        }

//        private double CalculateMomentumFactor(Team team1, Team team2)
//        {
//            var team1Recent = GetRecentWins(team1, 5);
//            var team2Recent = GetRecentWins(team2, 5);
//            var headToHead = GetHeadToHeadRecord(team1, team2);

//            // Calculate momentum (1.0 = neutral)
//            double momentum = 1.0;
//            momentum += (team1Recent.Wins - team2Recent.Wins) * 0.05;
//            momentum += (headToHead.Team1Wins - headToHead.Team2Wins) * 0.03;

//            return Math.Clamp(momentum, 0.8, 1.2);
//        }


//        private void AdvanceTournamentStage(Tournament tournament)
//        {
//            switch (tournament.Format)
//            {
//                case TournamentFormat.SingleElimination:
//                    AdvanceSingleElimination(tournament);
//                    break;

//                    // Implement other formats similarly
//            }
//        }

//        private void AdvanceSingleElimination(Tournament tournament)
//        {
//            string currentStage = tournament.CurrentStage;

//            var currentWinners = tournament.Matches
//                .Where(m => m.Stage == currentStage)
//                .Select(m => m.Winner)
//                .ToList();

//            if (currentWinners.Count == 1)
//            {
//                CompleteTournament(tournament);
//                return;
//            }

//            string nextStage = GetNextStage(currentStage);

//            for (int i = 0; i < currentWinners.Count; i += 2)
//            {
//                if (i + 1 < currentWinners.Count)
//                {
//                    tournament.Matches.Add(new Match
//                    {
//                        Team1 = currentWinners[i],
//                        Team2 = currentWinners[i + 1],
//                        Tournament = tournament,
//                        MatchDate = CalculateMatchDate(tournament, GetStageNumber(nextStage)),
//                        Stage = nextStage,
//                        IsDeciderMatch = (nextStage == "Grand Final")
//                    });
//                }
//                else
//                {
//                    // Handle odd number of winners (shouldn't happen in single elim)
//                    currentWinners[i].Budget += tournament.PrizePool * 0.05m;
//                }
//            }

//            tournament.CurrentStage = nextStage;
//        }

//        private int GetStageRoundNumber(string stage)
//        {
//            if (stage.StartsWith("Round of"))
//            {
//                // Extract the number from the stage name
//                if (int.TryParse(stage.Replace("Round of", "").Trim(), out int number))
//                {
//                    return number;
//                }
//            }
//            else if (stage == "Semifinals")
//            {
//                return 4;
//            }
//            else if (stage == "Final" || stage == "Grand Final")
//            {
//                return 2;
//            }

//            return 0; // fallback
//        }

//        private int GetStageNumber(string stage)
//        {
//            return stage switch
//            {
//                "Round of 64" => 1,
//                "Round of 32" => 2,
//                "Round of 16" => 3,
//                "Quarterfinals" => 4,
//                "Semifinals" => 5,
//                "Grand Final" => 6,
//                _ => 1 // Default to first stage
//            };
//        }

//        private string GetNextStage(string currentStage)
//        {
//            return currentStage switch
//            {
//                "Round of 64" => "Round of 32",
//                "Round of 32" => "Round of 16",
//                "Round of 16" => "Quarterfinals",
//                "Quarterfinals" => "Semifinals",
//                "Semifinals" => "Grand Final",
//                _ => "Finals"
//            };
//        }

//        private void GenerateInitialMatches(Tournament tournament)
//        {
//            tournament.Matches.Clear();

//            switch (tournament.Format)
//            {
//                case TournamentFormat.SingleElimination:
//                    GenerateSingleEliminationMatches(tournament);
//                    break;

//                case TournamentFormat.DoubleElimination:
//                    GenerateDoubleEliminationMatches(tournament);
//                    break;

//                case TournamentFormat.RoundRobin:
//                    GenerateRoundRobinMatches(tournament);
//                    break;

//                case TournamentFormat.Swiss:
//                    GenerateSwissMatches(tournament);
//                    break;
//            }
//        }
//        private void GenerateNextRoundMatches(Tournament tournament)
//        {
//            // Simple implementation - just pair winners from previous matches
//            var winners = tournament.Matches
//                .Where(m => m.IsCompleted)
//                .Select(m => m.Winner)
//                .Distinct()
//                .OrderBy(t => Guid.NewGuid())
//                .ToList();

//            tournament.Matches.Clear(); // Clear old matches

//            for (int i = 0; i < winners.Count; i += 2)
//            {
//                if (i + 1 < winners.Count)
//                {
//                    tournament.Matches.Add(new Match
//                    {
//                        Team1 = winners[i],
//                        Team2 = winners[i + 1],
//                        Tournament = tournament,
//                        MatchDate = new DateTime(_gameState.CurrentYear, 1, 1).AddDays((_gameState.CurrentWeek - 1) * 7)
//                    });
//                }
//                else
//                {
//                    // Handle odd number of teams (give a bye)
//                    winners[i].Budget += tournament.PrizePool * 0.1m; // Small prize for bye
//                }
//            }
//        }

//        private void GenerateSingleEliminationMatches(Tournament tournament)
//        {
//            GenerateSingleEliminationMatches(tournament, tournament.ParticipatingTeams);
//        }

//        private void GenerateSingleEliminationMatches(Tournament tournament, List<Team> teams)
//        {
//            // Check if we need byes (when team count isn't power of 2)
//            int teamsNeeded = (int)Math.Pow(2, Math.Ceiling(Math.Log(teams.Count, 2)));
//            int byes = teamsNeeded - teams.Count;

//            // Create first round matches
//            for (int i = 0; i < teams.Count - byes; i += 2)
//            {
//                tournament.Matches.Add(new Match
//                {
//                    Team1 = teams[i],
//                    Team2 = teams[i + 1],
//                    Tournament = tournament,
//                    MatchDate = CalculateMatchDate(tournament, 0),
//                    Stage = "Round of " + teamsNeeded,
//                    IsDeciderMatch = false
//                });
//            }

//            // Teams with byes automatically advance
//            for (int i = 0; i < byes; i++)
//            {
//                int teamIndex = teams.Count - byes + i;
//                tournament.Matches.Add(new Match
//                {
//                    Team1 = teams[teamIndex],
//                    Team2 = null, // Indicates a bye
//                    Tournament = tournament,
//                    MatchDate = CalculateMatchDate(tournament, 0),
//                    Stage = "Round of " + teamsNeeded,
//                    IsDeciderMatch = false,
//                    IsCompleted = true,
//                    Winner = teams[teamIndex]
//                });
//            }

//            tournament.CurrentStage = "Round of " + teamsNeeded;
//        }

//        private DateTime CalculateMatchDate(Tournament tournament, int round)
//        {
//            return new DateTime(tournament.Year, 1, 1)
//                .AddDays((tournament.Week - 1) * 7)
//                .AddDays(round * 2); // Matches every 2 days during tournament
//        }

//        private void GenerateDoubleEliminationMatches(Tournament tournament)
//        {
//            // Implement double elimination logic
//            // Similar to single elimination but with loser's bracket
//            throw new NotImplementedException("Double elimination not yet implemented");
//        }

//        private void GenerateSwissMatches(Tournament tournament)
//        {
//            // Implement Swiss system pairing
//            throw new NotImplementedException("Swiss system not yet implemented");
//        }

//        private double GetMapPerformanceFactor(Team team, string map)
//        {
//            if (team.MapWinRates.TryGetValue(map, out double winRate))
//            {
//                return 0.8 + (winRate * 0.4); // Convert to 0.8-1.2 factor
//            }
//            return 1.0; // Neutral if no data
//        }

//        private void GeneratePlayerPerformances(Match match)
//        {
//            if (match.Team1 == null || match.Team2 == null) return;

//            foreach (var player in match.Team1.Players.Concat(match.Team2.Players))
//            {
//                double performanceFactor = (player.Stats.Rating / 2.0) * (random.NextDouble() * 0.4 + 0.8);

//                player.MatchHistory.Add(new MatchHistory
//                {
//                    PlayerId = player.Id,
//                    MatchId = match.Id,
//                    Kills = (int)(15 * performanceFactor),
//                    Deaths = (int)(20 * (1.3 - performanceFactor)),
//                    Assists = (int)(8 * performanceFactor),
//                    Rating = Math.Round(performanceFactor * 1.5, 2),
//                    ADR = (int)(80 * performanceFactor),
//                    FlashAssists = random.Next(0, 5),
//                    EntryKills = random.Next(0, 4),
//                    Map = match.Map
//                });
//            }
//        }

//        private (int Wins, int Losses) GetRecentWins(Team team, int matchCount)
//        {
//            var recentMatches = _gameState.CompletedMatches
//                .Where(m => (m.Team1 == team || m.Team2 == team))
//                .OrderByDescending(m => m.MatchDate)
//                .Take(matchCount)
//                .ToList();

//            int wins = recentMatches.Count(m => m.Winner == team);
//            return (wins, recentMatches.Count - wins);
//        }

//        private HeadToHeadRecord GetHeadToHeadRecord(Team team1, Team team2)
//        {
//            var matches = _gameState.CompletedMatches
//                .Where(m => (m.Team1 == team1 && m.Team2 == team2) ||
//                           (m.Team1 == team2 && m.Team2 == team1))
//                .ToList();

//            return new HeadToHeadRecord
//            {
//                Team1 = team1,
//                Team2 = team2,
//                Team1Wins = matches.Count(m => m.Winner == team1),
//                Team2Wins = matches.Count(m => m.Winner == team2),
//                Draws = matches.Count(m => m.Team1Score == m.Team2Score)
//            };
//        }

//        private void CompleteTournament(Tournament tournament)
//        {
//            tournament.IsCompleted = true;
//            tournament.Winner = tournament.Matches.LastOrDefault()?.Winner;

//            // Distribute prize money
//            if (tournament.Winner != null)
//            {
//                DistributePrizeMoney(tournament);
//            }

//            _gameState.ActiveTournaments.Remove(tournament);
//            _gameState.CompletedTournaments.Add(tournament);

//            // Update team rankings
//            UpdateTeamRankings();
//        }

//        private void ProcessGroupStageMatches(Tournament tournament)
//        {
//            foreach (var group in tournament.GroupStages)
//            {
//                foreach (var match in group.Matches.Where(m =>
//                    !m.IsCompleted &&
//                    ShouldPlayMatchThisWeek(m.MatchDate)))
//                {
//                    PlayMatch(match);
//                }
//            }
//        }

//        private void ProcessPlayoffMatches(Tournament tournament)
//        {
//            foreach (var match in tournament.Matches.Where(m =>
//                !m.IsCompleted &&
//                ShouldPlayMatchThisWeek(m.MatchDate)))
//            {
//                PlayMatch(match);
//            }
//        }

//        private void GeneratePlayoffMatches(Tournament tournament)
//        {
//            var advancingTeams = tournament.GroupStages
//                .SelectMany(g => g.GetAdvancingTeams(tournament.TeamsAdvancingPerGroup))
//                .OrderBy(t => Guid.NewGuid()) // Randomize for bracket
//                .ToList();

//            GenerateSingleEliminationMatches(tournament, advancingTeams);
//        }

//        private DateTime CalculateGroupStageMatchDate(string groupName, int teamIndex1, int teamIndex2)
//        {
//            // Spread matches evenly across group stage duration
//            int matchIndex = (teamIndex1 * 10) + teamIndex2; // Simple hash
//            int daysOffset = matchIndex % 7; // Spread over a week
//            return new DateTime(_gameState.CurrentYear, 1, 1)
//                .AddDays((_gameState.CurrentWeek - 1) * 7)
//                .AddDays(daysOffset);
//        }

//        private bool ShouldPlayMatchThisWeek(DateTime matchDate)
//        {
//            return matchDate.Year == _gameState.CurrentYear &&
//                   matchDate.DayOfYear / 7 + 1 == _gameState.CurrentWeek;
//        }

//        private void PlayMatch(Match match)
//        {
//            if (match.Team2 == null) return; // Bye match

//            double team1Strength = CalculateTeamStrength(match.Team1);
//            double team2Strength = CalculateTeamStrength(match.Team2);

//            // Apply map-specific adjustments (you could add map property to Match)
//            team1Strength *= GetMapPerformanceFactor(match.Team1, "de_dust2");
//            team2Strength *= GetMapPerformanceFactor(match.Team2, "de_dust2");

//            // Add realistic randomness and momentum
//            double momentumFactor = CalculateMomentumFactor(match.Team1, match.Team2);
//            team1Strength *= momentumFactor;
//            team2Strength *= (2 - momentumFactor); // Inverse for other team

//            // Add clutch factor for close matches
//            double clutchFactor = 1 + (random.NextDouble() * 0.2 - 0.1); // ±10%

//            // Calculate round scores (MR15)
//            int team1Rounds = (int)(16 * (team1Strength / (team1Strength + team2Strength)) * clutchFactor);
//            int team2Rounds = 16 - team1Rounds;

//            // Ensure minimum 1 round difference
//            if (team1Rounds == team2Rounds)
//            {
//                if (random.Next(2) == 0) team1Rounds++;
//                else team2Rounds++;
//            }

//            match.Team1Score = Math.Min(16, team1Rounds);
//            match.Team2Score = Math.Min(16, team2Rounds);
//            match.IsCompleted = true;
//            match.Winner = team1Rounds > team2Rounds ? match.Team1 : match.Team2;

//            GeneratePlayerPerformances(match);
//            UpdateTeamMomentum(match);
//        }

//        private double CalculateTeamStrength(Team team)
//        {
//            if (team.Players.Count == 0) return 0;

//            double totalStrength = 0;
//            double roleSynergyBonus = 1.0;
//            var roles = team.Players.Select(p => p.Role).ToList();

//            foreach (var player in team.Players)
//            {
//                double playerStrength = player.Stats.Rating;

//                switch (player.Role)
//                {
//                    case PlayerRole.IGL:
//                        playerStrength *= 1 + (player.Stats.GameSense * 0.05);
//                        break;
//                    case PlayerRole.AWPer:
//                        playerStrength *= 1 + (player.Stats.Aim * 0.03 - player.Stats.Reflexes * 0.01);
//                        break;
//                    case PlayerRole.Support:
//                        playerStrength *= 1 + (player.Stats.Teamwork * 0.04);
//                        break;
//                    case PlayerRole.Lurker:
//                        playerStrength *= 1 + (player.Stats.Consistency * 0.04);
//                        break;
//                    case PlayerRole.Rifler:
//                        playerStrength *= 1 + (player.Stats.Aim * 0.04);
//                        break;
//                }

//                totalStrength += playerStrength;
//            }

//            // Synergy bonuses
//            if (roles.Contains(PlayerRole.IGL) && roles.Contains(PlayerRole.Support))
//                roleSynergyBonus *= 1.15;

//            if (roles.Count(r => r == PlayerRole.Rifler) >= 2)
//                roleSynergyBonus *= 1.05;

//            double averageStrength = totalStrength / team.Players.Count;
//            return averageStrength * roleSynergyBonus;
//        }

//        public void UpdateTeamRankings()
//        {
//            var allTeams = _gameState.AllTeams;
//            var allMatches = GetAllCompletedMatches();

//            // Calculate team scores with the new algorithm
//            var teamScores = allTeams.Select(team =>
//            {
//                double compositionScore = CalculateTeamCompositionScore(team);
//                double strengthScore = CalculateTeamStrength(team);
//                double performanceScore = CalculateRecentPerformance(team, allMatches);

//                return new
//                {
//                    Team = team,
//                    TotalScore = compositionScore * strengthScore * performanceScore
//                };
//            })
//            .OrderByDescending(t => t.TotalScore)
//            .ToList();

//            // Assign rankings based on the new scores
//            for (int i = 0; i < teamScores.Count; i++)
//            {
//                teamScores[i].Team.WorldRanking = i + 1;
//                teamScores[i].Team.TeamScore = teamScores[i].TotalScore; // Store for reference
//            }
//        }
//    }
//}
