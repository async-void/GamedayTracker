using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Media
    {
        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }
    }
}
