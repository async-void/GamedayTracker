using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Regulation
    {
        [JsonPropertyName("periods")]
        public int Periods { get; set; }
    }
}
