using System.Text.Json.Serialization;

namespace GamedayTracker.Models.News
{
    public class WebLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }
}
