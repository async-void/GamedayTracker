using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class BroadcastType
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }
    }
}
