using System.Text.Json.Serialization;

namespace GamedayTracker.Models.NFL
{
    public class Note
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
