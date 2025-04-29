using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class GroupStage
    {
        public string Name { get; set; }  // e.g., "Group A", "Group B"
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<Match> Matches { get; set; } = new List<Match>();
        public Dictionary<Team, int> Standings { get; set; } = new Dictionary<Team, int>();

        public void CalculateStandings()
        {
            // Reset standings
            Standings.Clear();
            foreach (var team in Teams)
            {
                Standings[team] = 0;
            }

            // Calculate points (3 for win, 1 for draw, 0 for loss)
            foreach (var match in Matches.Where(m => m.IsCompleted))
            {
                if (match.Team1Score > match.Team2Score)
                {
                    Standings[match.Team1] += 3;
                }
                else if (match.Team1Score == match.Team2Score)
                {
                    Standings[match.Team1] += 1;
                    Standings[match.Team2] += 1;
                }
                else
                {
                    Standings[match.Team2] += 3;
                }
            }

            // Sort by points (descending)
            Standings = Standings.OrderByDescending(kv => kv.Value)
                                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public List<Team> GetAdvancingTeams(int count)
        {
            return Standings.Keys.Take(count).ToList();
        }
    }
}
