using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class MatchHistory
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public double Rating { get; set; }
        public double ADR { get; set; } // Average Damage per Round
        public int FlashAssists { get; set; }
        public int EntryKills { get; set; }
    }
}
