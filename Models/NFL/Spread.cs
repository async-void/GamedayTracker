using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Spread
    {
        [JsonPropertyName("away")]
        public double? Away { get; set; }

        [JsonPropertyName("home")]
        public double? Home { get; set; }
    }
}
