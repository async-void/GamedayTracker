using System.Text.Json.Serialization;

namespace GamedayTracker.Models.News
{
    public class ArticleLinks
    {
        [JsonPropertyName("web")]
        public WebLink Web { get; set; }
    }
}
