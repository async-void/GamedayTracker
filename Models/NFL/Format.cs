using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Format
    {
        [JsonPropertyName("regulation")]
        public Regulation Regulation { get; set; }
    }
}
