using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Conference
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("standings")]
        public StandingsData? Standings { get; set; }
    }
}
