using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Broadcast
    {
        [JsonPropertyName("market")]
        public string Market { get; set; }

        [JsonPropertyName("names")]
        public List<string> Names { get; set; }
    }
}
