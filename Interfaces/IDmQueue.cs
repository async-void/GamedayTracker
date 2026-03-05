using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IDmQueue
    {
        void Enqueue(object payload);
        bool TryDequeue(out object? payload);
    }
}
