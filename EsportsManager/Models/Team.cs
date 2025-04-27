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
        public int WorldRanking { get; set; }
        public List<MatchHistory> MatchHistory { get; set; } = new();
    }
}
