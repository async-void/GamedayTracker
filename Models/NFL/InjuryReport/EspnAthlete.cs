using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL.InjuryReport
{
    public class EspnAthlete
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("position")]
        public EspnPosition? Position { get; set; }

        [JsonPropertyName("headshot")]
        public EspnHeadshot? Headshot { get; set; }
    }


}
