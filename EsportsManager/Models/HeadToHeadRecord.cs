using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class HeadToHeadRecord
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public int Team1Wins { get; set; }
        public int Team2Wins { get; set; }
        public int Draws { get; set; }
    }
}
