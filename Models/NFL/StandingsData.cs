using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class StandingsData
    {
        [JsonPropertyName("entries")]
        public List<StandingEntry>? Entries { get; set; }
    }
}
