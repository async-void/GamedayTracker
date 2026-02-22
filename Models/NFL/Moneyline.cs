using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Moneyline
    {
        [JsonPropertyName("away")]
        public int? Away { get; set; }

        [JsonPropertyName("home")]
        public int? Home { get; set; }
    }
}
