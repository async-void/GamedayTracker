using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class TeamStanding
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public required string Division { get; set; }
        public required string TeamName { get; set; }
        public required string Abbr { get; set; }
        public required string Wins { get; set; }
        public required string Loses { get; set; }
        public required string Ties { get; set; }
        public required string Pct { get; set; }
    }
}
