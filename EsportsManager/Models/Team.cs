using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public List<Player> Players { get; set; } = new();
        //public List<Player> Bench { get; set; } = new();
        public ObservableCollection<Player> Players { get; set; } = new();
        public ObservableCollection<Player> Bench { get; set; } = new();
        public decimal Budget { get; set; } = 100000; // Starting budget
        public decimal WeeklyExpense { get; set; } = 0;
        public int WorldRanking { get; set; }
        public List<MatchHistory> MatchHistory { get; set; } = new();
        public Color Color { get; set; } = new Color(255, 255, 255);
        public double TeamScore { get; set; } = 0;
        public string Region { get; set; }

        public double Momentum { get; set; } = 1.0;
        public Dictionary<string, double> MapWinRates { get; set; } = new();

        public void AddMapPerformance(string map, bool won)
        {
            if (!MapWinRates.ContainsKey(map))
                MapWinRates[map] = 0.5; // Default 50% win rate

            MapWinRates[map] = (MapWinRates[map] * 0.9) + (won ? 0.1 : 0);
        }

        private double GetMapPerformanceFactor(Team team, string map)
        {
            if (team.MapWinRates.TryGetValue(map, out double winRate))
            {
                return 0.8 + (winRate * 0.4); // Convert 0-1 winrate to 0.8-1.2 factor
            }
            return 1.0; // Neutral if no data
        }
        public void UpdateMapPerformance(string map, bool won)
        {
            if (!MapWinRates.ContainsKey(map))
                MapWinRates[map] = 0.5; // Default 50% win rate

            MapWinRates[map] = (MapWinRates[map] * 0.9) + (won ? 0.1 : 0);
        }
    }
}
