using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Enums;

namespace GamedayTracker.Models
{
    public class TeamStats
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public LineType LineType { get; set; }
        public string? TeamName { get; set; }
        public int GamesPlayed { get; set; }
        public int TotalPoints { get; set; }
        public double PointsPerGame { get; set; }
        public int RushYardsTotal { get; set; }
        public double RushPerGame { get; set; }
        public int PassYardsTotal { get; set; }
        public double PassYardsPerGame { get; set; }
        public int TotalYards { get; set; }
        public double YardsPerGame { get; set; }
    }
}
