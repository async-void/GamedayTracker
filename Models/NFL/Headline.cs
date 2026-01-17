using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Headline
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("shortLinkText")]
        public string ShortLinkText { get; set; }
    }
}
