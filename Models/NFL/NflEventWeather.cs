using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.NFL
{
    public sealed class NflEventWeather
    {
        [JsonPropertyName("displayValue")]
        public string? DisplayValue { get; set; }
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }

    }
}
