using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.News
{
    public class NewsSlashCommand: ApplicationCommandModule
    {
        [SlashCommand("news", "gets the most recent NFL news and updates.")]
        public async Task GetNewOrUpdates(InteractionContext ctx)
        {

        }
    }
}
