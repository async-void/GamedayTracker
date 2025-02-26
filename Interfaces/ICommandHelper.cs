using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Helpers;
using GamedayTracker.Models;

namespace GamedayTracker.Interfaces
{
    public interface ICommandHelper
    {
        Result<List<GuildMember>, SystemError<SlashCommandHelper>> BuildLeaderboard(string guildId, int scope);
    }
}
