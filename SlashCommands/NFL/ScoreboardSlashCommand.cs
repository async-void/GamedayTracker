using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.NFL;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GamedayTracker.SlashCommands.NFL
{
    public class ScoreboardSlashCommand(IGameData gameService, IDiscordEmbedService embedService)
    {

        [Command("scoreboard")]
        [Description("get the scores for a specified season & week")]
        public async Task GetScoreboard(SlashCommandContext ctx, NFLSeasonType seasonType, [SlashChoiceProvider<SeasonChoiceProvider>] int? season = null,
            [SlashChoiceProvider<WeekChoiceProvider>] int? week = null)
        {
            await ctx.DeferResponseAsync();

            var scores = await gameService.GetNFLScoresAsync(season, week, (int)seasonType);
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var embed = await embedService.CreateScoresEmbed(scores);
            var msg = new DiscordMessageBuilder()
                .AddEmbed(embed)
                .WithContent($"-# Gameday Tracker ©️ {unixTimestamp}");

            await ctx.RespondAsync(msg);

        }

        [Command("team-scoreboard")]
        [Description("get the season scores for a specific team")]
        public async Task GetTeamScoreboard(SlashCommandContext ctx, string teamName, int? season = null)
        {
            await ctx.DeferResponseAsync();

            if (season is null || season == 0)
                season = DateTimeOffset.UtcNow.Year;

            var scoreboard = await gameService.GetNFLScoresAsync(season);
            var teamAbbr = teamName.ToAbbr();
            var teamScoreboard = gameService.GetTeamGames(scoreboard, teamAbbr);

            DiscordComponent[] components =
            [
                new DiscordTextDisplayComponent($"{teamName}'s {season} Scoreboard")
            ];

            var container = new DiscordContainerComponent(components, false, new DiscordColor(1, 22, 33));
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            await ctx.RespondAsync(msg);
                
        }
    }
}
