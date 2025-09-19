using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Extensions;
using GamedayTracker.Services;
using GamedayTracker.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.SlashCommands.NFL
{
    public class ListTeamsSlashCommand
    {
        #region LIST TEAMS
        [Command("teams")]
        [Description("Lists all NFL teams.")]
        public async ValueTask ListTeamsAsync(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();

            #region TEAMS
            var afc_east = new string[]
            {
                "Buffalo",
                "Miami",
                "New England",
                "NY Jets"
            };
            var afc_north = new string[]
            {
                "Baltimore",
                "Cincinnati",
                "Cleveland",
                "Pittsburgh"
            };
            var afc_south = new string[]
            {
                "Houston",
                "Indianapolis",
                "Jacksonville",
                "Tennessee"
            };
            var afc_west = new string[]
            {
                "Denver",
                "Kansas City",
                "Las Vegas",
                "LA Chargers"
            };
            var nfc_east = new string[]
            {
                "Dallas",
                "NY Giants",
                "Philadelphia",
                "Washington"
            };
            var nfc_north = new string[]
            {
                "Chicago",
                "Detroit",
                "Green Bay",
                "Minnesota"
            };
            var nfc_south = new string[]
            {
                "Atlanta",
                "Carolina",
                "New Orleans",
                "Tampa Bay"
            };
            var nfc_west = new string[]
            {
                "Arizona",
                "LA Rams",
                "San Francisco",
                "Seattle"
            };
            #endregion

            var descBuilder = new StringBuilder();

            #region TEAM BUILDER
            descBuilder.AppendLine("**AFC East** <:afc:1331745347285811300>");
            foreach (var team in afc_east)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            descBuilder.AppendLine("**AFC North** <:afc:1331745347285811300>");
            foreach (var team in afc_north)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            descBuilder.AppendLine("**AFC South** <:afc:1331745347285811300>");
            foreach (var team in afc_south)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            descBuilder.AppendLine("**AFC West** <:afc:1331745347285811300>");
            foreach (var team in afc_west)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }

            descBuilder.AppendLine("**NFC East** <:nfc:1331741091636056196>");
            foreach (var team in nfc_east)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            descBuilder.AppendLine("**NFC North** <:nfc:1331741091636056196>");
            foreach (var team in nfc_north)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            descBuilder.AppendLine("**NFC South** <:nfc:1331741091636056196>");
            foreach (var team in nfc_south)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            descBuilder.AppendLine("**NFC West** <:nfc:1331741091636056196>");
            foreach (var team in nfc_west)
            {
                var teamAbbr = team.ToAbbr();
                var data = String.Format("{0, 3} | {1, 3}", teamAbbr, team);
                descBuilder.AppendLine($"- {NflEmojiService.GetEmoji(teamAbbr)} ``{data}``");
            }
            #endregion

            DiscordComponent[] comps =
                [
                    new DiscordTextDisplayComponent("Team List <:nfl:1331742015809130629>"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent(descBuilder.ToString()),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# Gameday Tracker ©️ {unixTimestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                ];
            var container = new DiscordContainerComponent(comps, false, DiscordColor.Gray);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            await ctx.EditResponseAsync(msg);
        }
        #endregion
    }
}
