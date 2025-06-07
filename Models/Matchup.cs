using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class Matchup
    {
        public int Id { get; set; }
        public required int Season { get; set; }
        public required int Week { get; set; }
        public string? GameTime { get; set; }
        public string? GameDate { get; set; }
        public Opponent? Opponents { get; set; }
    }
}
