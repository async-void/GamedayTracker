using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class NotificationPayload
    {
        public string Message { get; set; } = string.Empty;
        public ulong GuildId { get; set; }
    }
}
