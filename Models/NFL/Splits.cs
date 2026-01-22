using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Splits
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonPropertyName("categories")]
        public List<StatCategory> Categories { get; set; }
    }
}
