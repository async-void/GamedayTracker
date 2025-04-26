using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Factories;

namespace GamedayTracker.SlashCommands.NFL
{
    public class StandingsSlashCommand
    {
        [Command("standings")]
        [Description("get season Team Standings")]
        public async Task GetStandings(CommandContext ctx,
            [Parameter("season")] string season)
        {
            await ctx.DeferResponseAsync();

            await using var db = new AppDbContextFactory().CreateDbContext();
            var standings = db.TeamStandings
                .Where(s => s.Season == int.Parse(season))
                .ToList()
                .OrderByDescending(x => int.Parse(x.Wins)).ToList();

            if (standings.Count < 1)
            {
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"no standing found for season {season}"));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
            else
            {
                var sBuilder = new StringBuilder();

                foreach (var standing in standings)
                {
                    if (standing.Abbr.Length == 2)
                        sBuilder.Append($"``{standing.Abbr.PadLeft(2).PadRight(2)}\t{standing.Wins.PadLeft(3)}\t{standing.Loses}\t{standing.Pct}``\r\n");
                    else
                        sBuilder.Append($"``{standing.Abbr.PadLeft(2)}\t{standing.Wins.PadLeft(2)}\t{standing.Loses}\t{standing.Pct}``\r\n");
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"**Standings for Season {season}**"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"__``Team\tW\tL\t Pct``__\r\n"),
                    new DiscordTextDisplayComponent(sBuilder.ToString()),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}")
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
