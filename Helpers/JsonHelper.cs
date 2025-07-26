using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public class JsonHelper
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            WriteIndented = true
        };
    }
}
