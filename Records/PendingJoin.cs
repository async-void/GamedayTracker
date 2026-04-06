using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Records
{
    public record PendingJoin(ulong UserId, ulong ThreadId, ulong TicketId, DateTimeOffset CreatedAt);
    
}
