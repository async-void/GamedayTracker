using System.ComponentModel;
using System.Linq;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.NFL
{
    public class StandingsSlashCommand(ITeamData teamDataService)
    {
        [Command("standings")]
        [Description("get season Team Standings")]
        public async Task GetStandings(SlashCommandContext ctx,
            [Parameter("season")] string season)
        {
            await ctx.DeferResponseAsync();

            var standings = await teamDataService.GetAllTeamStandings(int.Parse(season));
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (!standings.IsOk)
            {
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"❌ ERROR ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent(standings.Error.ErrorMessage!),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.DarkRed);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            else
            {
                var sBuilder = new StringBuilder(); 
                var sorted = standings.Value.OrderByDescending(x => int.Parse(x.Wins)).ToList();
                foreach (var standing in sorted)
                {
                    var emoji = NflEmojiService.GetEmoji(standing.Abbr);
                    if (standing.Abbr.Length == 2)
                        sBuilder.Append($"{emoji} `{standing.Abbr, -3} {standing.Wins, 4} {standing.Loses, 4} {standing.Pct, 7}`");
                    else
                        sBuilder.Append($"{emoji}`{standing.Abbr.PadLeft(2, ' ')}\t{standing.Wins.PadLeft(2)}\t{standing.Loses}\t{standing.Pct}`\r\n");
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"**Standings for Season {season}**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent("__`Team\t W\t L\tPct`__"),
                    new DiscordTextDisplayComponent(sBuilder.ToString()),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                                            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Cyan);
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                    
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }

        }
    }
}
