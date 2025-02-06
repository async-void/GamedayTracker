using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IInteractionProvider
    {
        Result<string, SystemError<InteractionDataProviderService>> ParseButtonId(string buttonId);
    }
}
