using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.Cache;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using GamedayTracker.Pagination;
using GamedayTracker.Services;
using GamedayTracker.Services.Espn;
using GamedayTracker.Utility;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.NFL
{
    public class ScoreboardSlashCommand(IGameData gameService, IDiscordEmbedService embedService, IEspnClient espnClient, IEvaluator evaluator)
    {
        private readonly IEspnClient _espnClient = espnClient;
        private readonly IEvaluator _evaluator = evaluator;

        [Command("scoreboard")]
        [Description("get the scores for a specified season & week")]
        public async Task GetScoreboard(SlashCommandContext ctx, [SlashChoiceProvider<SeasonTypeChoiceProvider>] int? seasonType = null, [SlashChoiceProvider<SeasonChoiceProvider>] int? season = null,
            [SlashChoiceProvider<WeekChoiceProvider>] int? week = null)
        {
            await ctx.DeferResponseAsync();

            var noParams =
                (!season.HasValue || season.Value == 0) &&
                (!week.HasValue || week.Value == 0) &&
                (!seasonType.HasValue || seasonType.Value == 0);

            string seasonStr = season?.ToString() ?? "";
            string weekStr = week?.ToString() ?? "";
            string typeStr = seasonType?.ToString() ?? "";

            var scores = noParams
                ? await _espnClient.GetScoreboardAsync("", "", "")
                : await _espnClient.GetScoreboardAsync(seasonStr, weekStr, typeStr);

            var now = DateTime.Now;
            var sType = _evaluator.Evaluate(DateTime.Now); 

            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var emoji = NflEmojiService.GetEmoji("default");

            var completedGames = gameService.GetCompletedGames(scores);
            var page = await embedService.CreateScoreboardPage(scores, emoji, season ?? 2025, 0);
            var totalPages = (int)Math.Ceiling(completedGames.Count / 4.0);
            
            var buttons = PaginationBuilder.CreateNavigationButtons(0, totalPages);
            page.AddActionRowComponent(new DiscordActionRowComponent(buttons));

            await ctx.RespondAsync(page);
            var response = await ctx.GetResponseAsync();
            var paginationData = new NFLScoreboardPaginationData
            {
                Scoreboard = scores,
                CurrentPage = 0,
                TotalPages = totalPages,
                SeasonType = NFLSeasonType.RegularSeason,
                Season = season ?? DateTimeOffset.UtcNow.Year,
                Emoji = emoji,
                UserId = ctx.User.Id,
                MessageId = response.Id,
            };
            PaginationCache.Store(response.Id, paginationData);
            
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(10));
                PaginationCache.Remove(response.Id);
            });
        }

        [Command("team-scoreboard")]
        [Description("get the season scores for a specific team")]
        public async Task GetTeamScoreboard(SlashCommandContext ctx, string teamName, [SlashChoiceProvider<SeasonChoiceProvider>] int? season = null)
        {
            await ctx.DeferResponseAsync();

            if (season is null || season == 0)
                season = DateTimeOffset.UtcNow.Year;

            var scoreboard = await gameService.GetNFLScoresAsync(season);

            if (scoreboard.Events is not null)
            {
                var teamAbbr = teamName.ToAbbr();
                var teamScoreboard = gameService.GetTeamGames(scoreboard, teamAbbr);
                var defaultEmoji = NflEmojiService.GetEmoji("");
                var date = teamScoreboard[0].Date.Value.Year;

                var components = new List<DiscordComponent>()
                {
                    new DiscordTextDisplayComponent($"{teamName}'s {season} Scoreboard")
                };
                   
                foreach (var game in teamScoreboard)
                {
                    
                    var awayTeamName = game.Competitions[0].Competitors[0].Team.Name;
                    var awayEmoji = NflEmojiService.GetEmoji(game.Competitions[0].Competitors[0].Team.Abbreviation ?? defaultEmoji);
                    var awayTeamScore = game.Competitions[0].Competitors[0].LineScores.Last();
                    var homeTeamName = game.Competitions[0].Competitors[0].Team.Name;
                    var homeTeamScore = game.Competitions[0].Competitors[1].LineScores.Last();
                    var lineScore = gameService.GetLineScores(game.Competitions[0].Competitors[0], game.Competitions[0].Competitors[1]);
                    var homeEmoji = NflEmojiService.GetEmoji(game.Competitions[0].Competitors[1].Team.Abbreviation ?? defaultEmoji);
                
                    components.Add(new DiscordTextDisplayComponent($"{date}: {awayEmoji} at {homeEmoji}"));
                    components.Add(new DiscordTextDisplayComponent($"{lineScore}"));
                }   
                var container = new DiscordContainerComponent(components, false, new DiscordColor(1, 22, 33));
                var msg = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.RespondAsync(msg);
            }
            else
                await ctx.RespondAsync($"No games found for {teamName} in {season}");
        }
    }
}
