using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL.InjuryReport
{
    public class EspnHeadshot
    {
        [JsonPropertyName("href")]
        public string? Href { get; set; }
    }


}
