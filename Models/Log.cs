using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Enums;

namespace GamedayTracker.Models
{
    public class Log
    {
        public int Id { get; set; }
        public required string LogMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public LogType LogType { get; set; }
        public LogTarget LogTarget { get; set; }

    }
}
