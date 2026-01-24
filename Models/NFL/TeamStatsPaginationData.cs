using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models.NFL
{
    public class TeamStatsPaginationData
    {
        public NflTeamStatisticsResponse TeamStats { get; set; }
        public string Emoji { get; set; }
        public NFLSeasonType SeasonType { get; set; }
        public int Season { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public ulong UserId { get; set; }
        public ulong MessageId { get; set; }
    }
}
