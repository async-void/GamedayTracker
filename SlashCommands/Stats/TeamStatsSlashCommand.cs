using System.ComponentModel;
using ChalkDotNET;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Extensions;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.Stats
{
    public class TeamStatsSlashCommand(ITeamData teamDataService)
    {
        [Command("teamstats")]
        [Description("Get Stats by Team : example [Buffalo, Pittsburgh, Miami]")]
        public async Task GetTeamStats(SlashCommandContext ctx, [SlashChoiceProvider<OffenseDefenseChoiceProvider>] int choice, 
            [Parameter("team"), Description("example: Buffalo or Pittsburgh")] string teamName, [Parameter("season")] int season)
        {
            await ctx.DeferResponseAsync();
            var isOffense = "";

            switch (choice)
            {
                case 0:
                    isOffense = "Offense";
                    break;
                case 1:
                    isOffense = "Defense";
                    break;
                default:
                    isOffense = "Defense";
                    return;
            }
            //var s = await teamDataService.GetStatsAsync(choice, season);
            var stats = await teamDataService.GetTeamStatsAsync(choice, season, teamName);

            if (stats.IsOk)
            {
                var teamAbbr = stats.Value.TeamName!.ToAbbr();
                var teamEmoji = NflEmojiService.GetEmoji(teamAbbr);
                var logoUrl = LogoPathService.GetLogoPath(teamAbbr);
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"{teamEmoji} **{stats.Value.TeamName!.ToFullName()}**"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"**{stats.Value.Season} {isOffense} Stats**\r\n``Games: {stats.Value.GamesPlayed}``\r" +
                        $"``Total Pts: {stats.Value.TotalPoints}``\r``Pts/G: {stats.Value.PointsPerGame}``\r``RushYds: {stats.Value.RushYardsTotal:#,##0}``\r" +
                        $"``RYds/G: {stats.Value.RushPerGame}``\r``PassYds: {stats.Value.PassYardsTotal:#,##0}``\r" +
                        $"``PYds/G: {stats.Value.PassYardsPerGame}``\r``Total Yds: {stats.Value.TotalYards:#,##0}``\r" +
                        $"``Yds/G: {stats.Value.YardsPerGame}``"), new DiscordThumbnailComponent(logoUrl)),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}"),  new DiscordButtonComponent(DiscordButtonStyle.Success, "donateId", "Donate")),
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkGray);
                var msgBuilder = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                await ctx.RespondAsync(msgBuilder);
                return;
            }

            Console.WriteLine(
                $"{Chalk.Yellow($"[{DateTimeOffset.UtcNow}]")} {Chalk.Yellow($"[Gameday Tracker]")} {Chalk.DarkBlue("[INFO]")} {Chalk.DarkGray($"{stats.Error.ErrorMessage}")}");
            await ctx.EditResponseAsync($"Error: {stats.Error.ErrorMessage}");
        }
    }
}
