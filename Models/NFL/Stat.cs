using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Stat
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("value")]
        public double Value { get; set; }
        [JsonPropertyName("displayValue")]
        public string? DisplayValue { get; set; }
        [JsonPropertyName("rankDisplayValue")]
        public string? RankDisplayValue { get; set; }
    }
}
