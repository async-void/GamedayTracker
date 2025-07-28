using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Utility;

namespace GamedayTracker.Services
{
    public class BettingDataServiceProvider(IGameData gameDataService, IJsonDataService jsonDataService): IBetting
    {
        private readonly IGameData _gameDataService = gameDataService;
        private readonly IJsonDataService _jsonDataService = jsonDataService;

        public async Task<Result<bool, SystemError<BettingDataServiceProvider>>> PlaceBet(Matchup matchup, Bet bet, GuildMember member)
        {
            //1. get current scoreboard data
            var games = _gameDataService.GetCurrentScoreboard();

            if (!games.IsOk)
            {
                return Result<bool, SystemError<BettingDataServiceProvider>>.Err(new SystemError<BettingDataServiceProvider>()
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("3996dbaf-2da8-45ae-9fad-e7e48fb0916b")),
                    ErrorCode = Guid.Parse("3996dbaf-2da8-45ae-9fad-e7e48fb0916b"),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = this,
                });
            }
            //1. a matchup must exist in the current scoreboard data

            //1. b the matchup must not have started
            var currentMatchup = games.Value.FirstOrDefault(g => g.Opponents.AwayTeam.Name.Equals(matchup.Opponents.AwayTeam.Name));
            //2. check to see if member has enough bank balance to place the bet
            var _member = await _jsonDataService.GetMemberFromJsonAsync(member.MemberId, member.GuildId);
            
            if (!_member.IsOk)// if member is null - return false.
            {
                return Result<bool, SystemError<BettingDataServiceProvider>>.Err(new SystemError<BettingDataServiceProvider>()
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("6b24c3ac-0a78-49fd-9c6e-40d749e6559e")),
                    ErrorCode = Guid.Parse("6b24c3ac-0a78-49fd-9c6e-40d749e6559e"),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = this,
                });
            }
            //3. if both are true return ok(true) otherwise return Err(new SystemError<BettingDataServiceProvider>);

            return Result<bool, SystemError<BettingDataServiceProvider>>.Ok(true);
        }

    }
}
