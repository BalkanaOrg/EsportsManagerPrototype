using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class SwissRecord
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public float BuchholzScore { get; set; } // Common Swiss tiebreaker
    }
}
