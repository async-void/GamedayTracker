using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class WeekInfo
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }
    }
}
