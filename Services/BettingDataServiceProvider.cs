using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Services
{
    public class BettingDataServiceProvider(IGameData gameDataService, IJsonDataService jsonDataService): IBetting
    {
        private readonly IGameData _gameDataService = gameDataService;
        private readonly IJsonDataService _jsonDataService = jsonDataService;

        public async Task<Result<bool, SystemError<BettingDataServiceProvider>>> IsValidBet(Matchup matchup, Bet bet, GuildMember member)
        {
            //1. check to see if member has enough bank balance to place the bet
            var _member = await _jsonDataService.GetMemberFromJsonAsync(member.MemberId, member.GuildId);
              // if member is null - return false.
              
            //2. check to see if matchup is valid

            //3. if both are true return ok(true) otherwise return Err(new SystemError<BettingDataServiceProvider>);

            return Result<bool, SystemError<BettingDataServiceProvider>>.Ok(true);
        }

    }
}
