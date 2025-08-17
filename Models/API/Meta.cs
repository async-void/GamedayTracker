using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.API
{
    public class Meta
    {
        [JsonPropertyName("per_page")]
        public int per_page { get; set; }

        [JsonPropertyName("next_cursor")]
        public int next_cursor { get; set; }
    }
}
