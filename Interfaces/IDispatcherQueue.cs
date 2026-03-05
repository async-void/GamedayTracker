using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IDispatcherQueue
    {
        void Enqueue(NotificationPayload payload);
        bool TryDequeue(out NotificationPayload? payloads);
    }
}
