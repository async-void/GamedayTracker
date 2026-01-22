using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.News
{
    public class NewsArticle
    {
        [JsonPropertyName("headline")]
        public string Headline { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("published")]
        public DateTime Published { get; set; }

        [JsonPropertyName("links")]
        public ArticleLinks Links { get; set; }

        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }
    }
}
