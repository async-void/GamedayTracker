using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Microsoft.VisualBasic;

namespace GamedayTracker.Interfaces
{
    public interface IInteractionProvider
    {
        Result<string, SystemError<InteractionDataProviderService>> ParseButtonId(string buttonId);

        Result<DiscordMessageBuilder, SystemError<InteractionDataProviderService>> BuildDiscordMessage(string title,
            string description);

        
    }
}
