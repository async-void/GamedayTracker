using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamedayTracker.Models.NFL.InjuryReport
{
    public class EspnInjuryResponse
    {
        [JsonPropertyName("injuries")]
        public List<EspnInjury>? Injuries { get; set; }
    }


}
