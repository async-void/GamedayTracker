using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class DraftEntity
    {
        public int Season { get; set; }
        public required string Round { get; set; }
        public required string PickPosition { get; set; }
        public required  string TeamName { get; set; }
        public required string PlayerName { get; set; }
        public required string Pos { get; set; }
        public required string College { get; set; }
    }
}
