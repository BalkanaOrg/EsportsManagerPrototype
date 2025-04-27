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
        public Team Winner => Team1Score > Team2Score ? Team1 : Team2;
    }

}
