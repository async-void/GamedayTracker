using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace GamedayTracker.SlashCommands.Settings
{
    [SlashCommandGroup("user-settings", "Gameday Tracker user-settings")]
    public class SettingsSlashCommands: ApplicationCommandModule
    {
        [SlashCommand("favorite-team", "set's the user's favorite NFL team.")]
        public async Task SetFavoriteTeam(InteractionContext ctx,
            [Option("team", "enter favorite team example KC or PIT")] string teamName)
        {
           
        }
    }
}
