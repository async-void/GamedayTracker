using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class LineScore
    {
        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("displayValue")]
        public string DisplayValue { get; set; }

        [JsonPropertyName("period")]
        public int Period { get; set; }
    }
}
