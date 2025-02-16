using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.SlashCommands.NFL
{
    [Command("Draft")]
    [Description("Draft group commands")]
    public class DraftSlashCommands
    {
        private readonly ITeamData _teamData = new TeamDataService();

        [Command("get")]
        [Description("Get supplied draft season")]
        public async Task GetDraftSeason(CommandContext ctx, [Parameter("season")] int season, [Parameter("team")] string teamName)
        {
            await ctx.DeferResponseAsync();
            await ctx.EditResponseAsync("your request is being processed...this may take a moment");

            var results = await _teamData.GetDraftResultsAsync(season);

            if (results.IsOk)
            {
                await ctx.EditResponseAsync($"I have the results you asked for {results.Value.Count}");
                var draft = results.Value.Where(x => x.TeamName.Equals(teamName)).ToList();

                //no results found..... notify the user!
                if (draft.Count == 0)
                {
                    await ctx.EditResponseAsync($"no results found for **{teamName}** in season **{season}**");
                    return;
                }

                var msgBuilder = new StringBuilder();

                //results found...notify the user!
                foreach (var draftEntity in draft)
                {
                    msgBuilder.Append($"round **{draftEntity.Round}** | **{draftEntity.PlayerName}** | position **{draftEntity.Pos}** | college **{draftEntity.College}**\r\n");
                }
                var draftMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"{teamName} {season} Draft Results")
                        .WithDescription(msgBuilder.ToString()));

                await ctx.EditResponseAsync(draftMessage);
            }
            else
            {
                var errorMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Error")
                        .WithDescription($"there was an error while fetching the draft for season {season}")
                        .AddField("Error", results.Error!.ErrorMessage!, true)
                        .AddField("Timestamp", results.Error!.CreatedAt!.ToString()!, true)
                        .AddField("Author", results.Error!.CreatedBy!.ToString()!, true));

                await ctx.EditResponseAsync(errorMessage);
            }
            
        }
    }
}
