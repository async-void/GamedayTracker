using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.API
{
    public class Root
    {
        [JsonPropertyName("data")]
        public List<Datum> data { get; set; }

        [JsonPropertyName("meta")]
        public Meta meta { get; set; }
    }
}
