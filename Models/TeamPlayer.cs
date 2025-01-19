using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class TeamPlayer
    {
        public int Id { get; set; }
        public required string PlayerName { get; set; }
        public required string TeamName { get; set; }
        public required string Position { get; set; }
        public required string College { get; set; }
        public required string Height { get; set; }
        public required string Weight { get; set; }
        public required string Age { get; set; }
        public required string Number { get; set; }
    }
}
