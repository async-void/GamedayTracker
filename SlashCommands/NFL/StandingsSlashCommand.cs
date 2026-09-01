using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Interfaces;
using System.ComponentModel;

namespace GamedayTracker.SlashCommands.NFL
{
    public class StandingsSlashCommand(ITeamData teamDataService, IGameData gameData, IDiscordEmbedService embedService)
    {
        [Command("standings")]
        [Description("get season Team Standings")]
        public async Task GetStandings(SlashCommandContext ctx,
            [SlashChoiceProvider<ConferenceChoiceProvider>] int conf)
        {
            await ctx.DeferResponseAsync();

            switch (conf)
            {
                case 0:
                    var embeds = await embedService.CreateStandingsEmbedsByConferenceAsync();
                    var msg = new DiscordMessageBuilder().AddEmbed(embeds.First());
                    await ctx.RespondAsync(msg);
                    break;
                case 1:
                    embeds = await embedService.CreateStandingsEmbedsByConferenceAsync();
                    msg = new DiscordMessageBuilder().AddEmbed(embeds.Last());
                    await ctx.RespondAsync(msg);
                    break;
                case 2:
                    var embed = await embedService.CreateStandingsEmbedAsync();
                    msg = new DiscordMessageBuilder().AddEmbed(embed);
                    await ctx.RespondAsync(msg);
                    break;
                default:
                    await ctx.RespondAsync("no standings found");
                    break;
            }
            
            

            //var testStandings = await gameData.GetNFLStandingsAsync();
            //var standings = await teamDataService.GetAllTeamStandings(season);
            //var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            //if (standings.IsOk && standings.Value.Count > 0)
            //{
            //    var sb = new StringBuilder();
            //    var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            //    var grouped = standings.Value
            //        .GroupBy(s => s.Division)
            //        .Select(standing => new
            //        {
            //            Division = standing.Key,
            //            Teams = standing.Select(t => new
            //            {
            //                t.TeamName,
            //                t.Wins,
            //                t.Loses,
            //                t.Pct
            //            })
            //        })
            //        .ToList();

            //    for (var i = 0; i < grouped.Count; i++)
            //    {
            //        sb.AppendLine($"-# {grouped[i].Division}");
            //        sb.AppendLine("__`Team\t W\t L\tPct`__");
            //        for (var j = 0; j < grouped[i].Teams.Count(); j++)
            //        {
            //            var abbr = grouped[i].Teams.ElementAt(j).TeamName.ToAbbr();
            //            var emoji = NflEmojiService.GetEmoji(abbr);
            //            sb.AppendLine($"{emoji} `{abbr,-3}:{grouped[i].Teams.ElementAt(j).Wins,4} {grouped[i].Teams.ElementAt(j).Loses,4} {grouped[i].Teams.ElementAt(j).Pct,7}`");
            //        }
            //    }
            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent($"## NFL Standings\r\n-# {season}"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordTextDisplayComponent($"{sb}"),
            //        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            //        new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Powered by GamedayTracker ©️ {timestamp}"),
            //            new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            //    ];
            //    var container = new DiscordContainerComponent(components, false, DiscordColor.DarkButNotBlack);
            //    var embed = new DiscordMessageBuilder()
            //        .EnableV2Components()
            //        .AddContainerComponent(container);
            //    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(embed));
            //}
            //else
            //{
            //    DiscordComponent[] components =
            //    [
            //        new DiscordTextDisplayComponent($"**ERROR**"),
            //        new DiscordSeparatorComponent(true),
            //        new DiscordTextDisplayComponent($"Could not find data for season {season}"),
            //        new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            //        new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}"),
            //                                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
            //    ];
            //    var container = new DiscordContainerComponent(components, false, DiscordColor.DarkRed);
            //    var message = new DiscordInteractionResponseBuilder()
            //        .EnableV2Components()
            //        .AddContainerComponent(container);

            //    await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            //}

        }
    }
}
