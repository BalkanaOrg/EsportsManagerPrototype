using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class Match
    {
        public int Id { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public DateTime MatchDate { get; set; }
        public Tournament Tournament { get; set; }
        public bool IsCompleted { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public string Stage { get; set; } // e.g., "Group A", "Quarterfinals"
        public bool IsDeciderMatch { get; set; }
        public string Map { get; set; } = "de_dust2";

        private Team _winner;
        public Team Winner
        {
            get => _winner ?? (Team1Score > Team2Score ? Team1 : Team2);
            set => _winner = value;
        }
    }

}
