using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Status
    {
        [JsonPropertyName("clock")]
        public double Clock { get; set; }

        [JsonPropertyName("displayClock")]
        public string DisplayClock { get; set; }

        [JsonPropertyName("period")]
        public int Period { get; set; }

        [JsonPropertyName("type")]
        public StatusType Type { get; set; }

        [JsonPropertyName("isTBDFlex")]
        public bool IsTBDFlex { get; set; }
    }
}
