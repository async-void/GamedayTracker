using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Athlete
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }

        [JsonPropertyName("links")]
        public List<Link> Links { get; set; }

        [JsonPropertyName("headshot")]
        public string Headshot { get; set; }

        [JsonPropertyName("jersey")]
        public string Jersey { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; }

        [JsonPropertyName("team")]
        public TeamReference Team { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}
