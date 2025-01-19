using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Division { get; set; }
        public required string Record { get; set; }
        public required string Abbreviation { get; set; }
        public required string LogoPath { get; set; }
        public string[]? TeamInfoLinks { get; set; }
        public int? Score { get; set; }
        public virtual ICollection<TeamPlayer>? Players { get; set; }
    }
}
