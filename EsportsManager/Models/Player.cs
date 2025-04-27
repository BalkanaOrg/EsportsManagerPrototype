using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Nationality { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public PlayerRole Role { get; set; }
        public PlayerStats Stats { get; set; }
        public int CurrentTeamId { get; set; } // 0 means free agent
        public bool IsBenched { get; set; }
        public decimal MarketValue { get; set; }
        public decimal Salary { get; set; }
        public decimal Buyout { get; set; } = 0;
        public int[] ContractExpiration { get; set; } = [0,0]; // The year and week in which a player's contract expires.
        public List<MatchHistory> MatchHistory { get; set; } = new();

        public string FullName => $"{FirstName} \"{Nickname}\" {LastName}";
    }
}
