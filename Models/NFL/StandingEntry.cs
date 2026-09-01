using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class StandingEntry
    {
        [JsonPropertyName("team")]
        public NFLTeam Team { get; set; }
        [JsonPropertyName("stats")]
        public List<Stat> Stats { get; set; }
    }
}
