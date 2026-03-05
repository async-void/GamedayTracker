using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IPaginationHandler
    {
        Task HandleNextAsync(object data, InteractionCreatedEventArgs eventArgs);
        Task HandlePreviousAsync(object data, InteractionCreatedEventArgs eventArgs);
    }
}
