using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportsManager.Models
{
    public class PlayerStats
    {
        public double Rating { get; set; } // 0-2.0 scale
        public double Aim { get; set; } // 0-100
        public double Reflexes { get; set; } // 0-100
        public double GameSense { get; set; } // 0-100
        public double Teamwork { get; set; } // 0-100
        public double Consistency { get; set; } // 0-100
        public double Potential { get; set; } // 0-100
    }
}
