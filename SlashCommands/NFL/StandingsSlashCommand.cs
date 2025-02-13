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
                sBuilder.Append($"__``Team\tW\tL\t Pct``__\r\n");

                foreach (var standing in standings)
                {
                    if (standing.Abbr.Length == 2)
                        sBuilder.Append($"``{standing.Abbr.PadLeft(2).PadRight(2)}\t{standing.Wins.PadLeft(3)}\t{standing.Loses}\t{standing.Pct}``\r\n");
                    else
                        sBuilder.Append($"``{standing.Abbr.PadLeft(2)}\t{standing.Wins.PadLeft(2)}\t{standing.Loses}\t{standing.Pct}``\r\n");
                }

                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Season {season} Standings")
                        .WithDescription(sBuilder.ToString()));

                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }

        }
    }
}
