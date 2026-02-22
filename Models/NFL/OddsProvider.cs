using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class OddsProvider
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
