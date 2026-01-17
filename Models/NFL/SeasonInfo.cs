using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class SeasonInfo
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }
    }
}
