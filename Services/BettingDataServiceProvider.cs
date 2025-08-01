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

        public async Task<Result<bool, SystemError<BettingDataServiceProvider>>> CanPlaceBet(Matchup matchup, Bet bet, GuildMember member)
        {
            //1. get current scoreboard data
            var week = _gameDataService.GetCurWeek();
            var season = _gameDataService.GetCurSeason();

            var games = await _gameDataService.GetCurrentScoreboard();

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
           
            //1. matchup must exist in the current scoreboard data
            var validMatchup = games.Value.FirstOrDefault(x => x.Opponents.AwayTeam.Name.Equals(matchup.Opponents.AwayTeam.Name, StringComparison.InvariantCultureIgnoreCase) 
            || x.Opponents.HomeTeam.Name.Equals(matchup.Opponents.HomeTeam.Name, StringComparison.InvariantCultureIgnoreCase));
           
            //1. a the matchup must not have started
            if (validMatchup is null)
                return Result<bool, SystemError<BettingDataServiceProvider>>.Err(new SystemError<BettingDataServiceProvider>()
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("3996dbaf-2da8-45ae-9fad-e7e48fb0916b")),
                    ErrorCode = Guid.Parse("3996dbaf-2da8-45ae-9fad-e7e48fb0916b"),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = this,
                });

            var now = DateTimeOffset.UtcNow;
            var gameTime = DateTimeOffset.Parse(validMatchup.GameTime!);
            var hasGameStarted = gameTime <= now;

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
            //3. member must not have the same bet twice, so check for multiple bets.
            var memberBets = member.Bets.FirstOrDefault(x => x.Matchup.Opponents.AwayTeam.Name.Equals(matchup.Opponents.AwayTeam.Name, StringComparison.OrdinalIgnoreCase) ||
                x.Matchup.Opponents.HomeTeam.Name.Equals(matchup.Opponents.HomeTeam.Name));
            if (memberBets is not null)
                return Result<bool, SystemError<BettingDataServiceProvider>>.Err(new SystemError<BettingDataServiceProvider>()
                {
                    ErrorMessage = SystemErrorCodes.GetErrorMessage(Guid.Parse("8857a4c9-1775-4e26-a4be-0a3cb20ff4dd")),
                    ErrorCode = Guid.Parse("8857a4c9-1775-4e26-a4be-0a3cb20ff4dd"),
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = this,
                });
            //4. if we get this far bet is valid
            var balance = _member.Value.Bank?.Balance ?? 0;
            return Result<bool, SystemError<BettingDataServiceProvider>>.Ok(true);
        }

    }
}
