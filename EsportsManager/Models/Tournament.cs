using System.ComponentModel;

namespace EsportsManager.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TournamentTier Tier { get; set; }
        public int PrizePool { get; set; }
        public List<Match> Matches { get; set; } = new();
        public int Year { get; set; }
        public int Week { get; set; } // Starting week
        public int DurationWeeks { get; set; } = 1;
        public Team Winner { get; set; }
        public bool IsCompleted { get; set; }

        private List<Team> _participatingTeams = new();
        public List<Team> ParticipatingTeams
        {
            get => _participatingTeams;
            set
            {
                _participatingTeams = value ?? new List<Team>();
                OnPropertyChanged(nameof(ParticipatingTeams)); // Implement INotifyPropertyChanged if needed
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TournamentFormat Format { get; set; }
        public List<GroupStage> GroupStages { get; set; } = new();
        public string CurrentStage { get; set; } = "1";
        public int TeamsPerGroup { get; set; }
        public int TeamsAdvancingPerGroup { get; set; }

        public bool HasGroupStage => GroupStages.Any();

        public List<Match> AllMatches
        {
            get
            {
                var allMatches = new List<Match>(Matches);
                foreach (var group in GroupStages)
                {
                    allMatches.AddRange(group.Matches);
                }
                return allMatches;
            }
        }
    }
}
