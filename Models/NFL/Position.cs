using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Position
    {
        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; }
    }
}
