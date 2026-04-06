using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL.InjuryReport
{
    public class EspnInjury
    {
        [JsonPropertyName("athlete")]
        public EspnAthlete? Athlete { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusDescription")]
        public string? StatusDescription { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("details")]
        public string? Details { get; set; }

        [JsonPropertyName("expectedReturn")]
        public string? ExpectedReturn { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("shortComment")]
        public string? ShortComment { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }
    }


}
