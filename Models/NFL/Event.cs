using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Event
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("shortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("season")]
        public SeasonInfo? Season { get; set; }

        [JsonPropertyName("week")]
        public WeekInfo? Week { get; set; }

        [JsonPropertyName("competitions")]
        public List<Competition>? Competitions { get; set; }

        [JsonPropertyName("links")]
        public List<Link>? Links { get; set; }

        [JsonPropertyName("status")]
        public Status? Status { get; set; }

        [JsonPropertyName("odds")]
        public List<Odds>? Odds { get; set; }

        [JsonPropertyName("weather")]
        public NflEventWeather? Weather { get; set; }
    }
}
