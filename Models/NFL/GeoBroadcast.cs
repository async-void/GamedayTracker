using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class GeoBroadcast
    {
        [JsonPropertyName("type")]
        public BroadcastType Type { get; set; }

        [JsonPropertyName("market")]
        public Market Market { get; set; }

        [JsonPropertyName("media")]
        public Media Media { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }
    }
}
