using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GamedayTracker.Factories;

namespace GamedayTracker.SlashCommands
{
    public class StandingsSlashCommand
    {
        [Command("standings")]
        [Description("get season Team Standings")]
        public async Task GetStandings(CommandContext ctx,
            [Option("season", "the season to fetch standings")] string season)
        {
            await ctx.DeferResponseAsync();

            await using var db = new AppDbContextFactory().CreateDbContext();
            var standings = db.TeamStandings
                .Where(s => s.Season == int.Parse(season))
                .ToList()
                .OrderByDescending(x => int.Parse(x.Wins));

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
