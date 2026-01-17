using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Market
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
