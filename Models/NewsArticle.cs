using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string? ImgUrl { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
}
