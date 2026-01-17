using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Leader
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("shortDisplayName")]
        public string ShortDisplayName { get; set; }

        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonPropertyName("leaders")]
        public List<LeaderDetail> Leaders { get; set; }
    }
}
