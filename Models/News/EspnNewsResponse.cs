using System.Text.Json.Serialization;

namespace GamedayTracker.Models.News
{
    public class EspnNewsResponse
    {
        [JsonPropertyName("articles")]
        public List<NewsArticle> Articles { get; set; }
    }
}
