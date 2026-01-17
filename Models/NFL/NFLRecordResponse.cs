using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.NFL
{
    public class NFLRecordResponse
    {
        [JsonPropertyName("team")]
        public NFLTeam Team { get; set; }
    }
}
