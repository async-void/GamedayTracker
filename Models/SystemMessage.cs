using GamedayTracker.Enums;
using GamedayTracker.ErrorInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models
{
    public class SystemMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public SystemErrorInfo? ErrorInfo {get; set;} = null;
        public MessageType MessageType { get; set; } = MessageType.Info;
        public required string Message { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
