using System.Text.Json.Serialization;

namespace GamedayTracker.Models
{
    public class TeamJson
    {
        [JsonPropertyName("abbr")]
        public string? Abbr { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }
    }
}
