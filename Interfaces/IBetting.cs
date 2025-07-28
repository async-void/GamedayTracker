using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IBetting
    {
        Task<Result<bool, SystemError<BettingDataServiceProvider>>> PlaceBet(Matchup matchup, Bet bet, GuildMember member);
    }
}
