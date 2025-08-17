using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GamedayTracker.SlashCommands.NFL
{
    public class StandingsSlashCommand(ITeamData teamDataService)
    {
        [Command("standings")]
        [Description("get season Team Standings")]
        public async Task GetStandings(SlashCommandContext ctx,
            [SlashChoiceProvider<SeasonChoiceProvider>] int season)
        {
            await ctx.DeferResponseAsync();

            var standings = await teamDataService.GetAllTeamStandings(season);
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
                    sBuilder.Append($"{emoji} `{standing.Abbr, -3} {standing.Wins, 4} {standing.Loses, 4} {standing.Pct, 7}`\r\n");
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
