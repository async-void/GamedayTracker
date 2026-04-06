using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL.InjuryReport
{
    public class EspnPosition
    {
        [JsonPropertyName("abbreviation")]
        public string? Abbreviation { get; set; }
    }


}
