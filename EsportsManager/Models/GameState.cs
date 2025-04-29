using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class GameState
    {
        public int CurrentYear { get; set; } = 2025;
        public int CurrentWeek { get; set; } = 1;
        public Team UserTeam { get; set; }
        public List<Team> AllTeams { get; set; } = new();
        public List<Player> FreeAgents { get; set; } = new();
        public List<Tournament> ActiveTournaments { get; set; } = new();
        public List<Tournament> UpcomingTournaments { get; set; } = new();
        public List<Tournament> CompletedTournaments { get; set; } = new();
        public List<Match> ActiveMatches { get; set; } = new();
        public List<Match> UpcomingMatches { get; set; } = new();
        public List<Match> CompletedMatches { get; set; } = new();
        public decimal Budget { get; set; } = 1000000;
        public decimal WeeklyExpense { get; set; } = 0;
    }
}
