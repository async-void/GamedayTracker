using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using GamedayTracker.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GamedayTracker.Services
{
    public class DiscordEmbedService(IGameData gameData): IDiscordEmbedService
    {
        #region CREATE LIVE GAME EMBED
        public DiscordEmbed CreateLiveGamesEmbed(List<Event> liveGames)
        {
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(255, 0, 0))
                .WithTitle($"🔴 LIVE NFL GAMES ({liveGames.Count})")
                .WithFooter($"Gameday Tracker : {unixTimestamp}");
                //.WithTimestamp(DateTimeOffset.UtcNow);

            foreach (var game in liveGames)
            {
                var gameInfo = gameData.GetGameInfo(game);
                var competition = game.Competitions[0];
                var leaders = gameData.GetGameLeaders(competition);

                embed.AddField(game.ShortName, $"{gameInfo}\n\n{leaders}", inline: false);
            }

            return embed.Build();
        }
        #endregion

        #region CREATE SCORES EMBED
        public async Task<DiscordEmbed> CreateScoresEmbed(NFLScoreboard data)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            //var seasonName = gameData.GetSeasonTypeName(data.Season.Type);
            var displayName = gameData.GetFullSeasonWeekDisplay(data);
            var titleEmoji = NflEmojiService.GetEmoji("NFL");

            if (data.Events == null || data.Events.Count == 0)
            {
                var errorEmbed = new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor(1, 51, 105))
                    .WithTitle($"{titleEmoji} NFL Scores\r\n{displayName}")
                    .WithFooter($"Gameday Tracker ")
                    .WithTimestamp(DateTimeOffset.UtcNow)
                    .WithDescription("No games scheduled for this week.");
                return errorEmbed.Build();
            }

            var sb = new StringBuilder();

            // Separate games by status
            var liveGames = gameData.GetLiveGames(data);
            var completedGames = gameData.GetCompletedGames(data);
            var scheduledGames = gameData.GetScheduledGames(data);

            // Add live games first
            if (liveGames.Count > 0)
            {
                sb.AppendLine("🔴 LIVE GAMES");
                sb.AppendLine("=================\r\n");
               // embed.AddField("🔴 LIVE GAMES", "---");
                foreach (var game in liveGames)
                {
                    var gameInfo = await gameData.GetGameInfo(game);
                    var seperators = new[] { "@", "VS" };
                    var names = game.ShortName.Split(seperators, StringSplitOptions.TrimEntries);
                    var awayEmoji = NflEmojiService.GetEmoji(names[0].Trim());
                    var homeEmoji = NflEmojiService.GetEmoji(names[1].Trim());
                    sb.AppendLine($"{awayEmoji} at {homeEmoji} : {gameInfo}");
                    //embed.AddField($"{awayEmoji} at {homeEmoji}", gameInfo, inline: false);
                }
                
            }

            // Add completed games
            if (completedGames.Count > 0)
            {
                sb.AppendLine("\r\n🔴 COMPLETED");
                sb.AppendLine("=================\r\n");
                //embed.AddField("✅ COMPLETED", "---");
                foreach (var game in completedGames)
                {
                    var gameInfo = await gameData.GetGameInfo(game);
                    var headline = game.Competitions[0].Headlines?.FirstOrDefault()?.ShortLinkText ?? "not found";
                    var seperators = new[] { "@", "VS" };
                    var names = game.ShortName.Split(seperators, StringSplitOptions.TrimEntries);
                    var awayEmoji = NflEmojiService.GetEmoji(names[0].Trim());
                    var homeEmoji = NflEmojiService.GetEmoji(names[1].Trim());
                    sb.AppendLine($"{awayEmoji} at {homeEmoji} : {gameInfo}");
                    sb.AppendLine($"📰 Headline {headline}");
                    //embed.AddField($"{awayEmoji} at {homeEmoji}", gameInfo, inline: false);
                    //embed.AddField("📰 Headline", headline, inline: false);
                }
            }

            // Add scheduled games
            if (scheduledGames.Count > 0)
            {
                sb.AppendLine("\r\n📅 SCHEDULED");
                sb.AppendLine("=================\r\n");
                //embed.AddField("📅 SCHEDULED", "---");
                foreach (var game in scheduledGames)
                {
                    var gameInfo = await gameData.GetGameInfo(game);
                    var seperators = new[] { "@", "VS" };
                    var names = game.ShortName.Split(seperators, StringSplitOptions.TrimEntries);
                    var awayEmoji = NflEmojiService.GetEmoji(names[0].Trim());
                    var homeEmoji = NflEmojiService.GetEmoji(names[1].Trim());
                    sb.AppendLine($"{awayEmoji} at {homeEmoji} : {gameInfo}");
                    //embed.AddField($"{awayEmoji} at {homeEmoji}", gameInfo, inline: false);
                }
            }
            DiscordComponent[] comps =
            [
                new DiscordTextDisplayComponent($"{titleEmoji} NFL Scores\r\n{displayName}"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent(sb.ToString()),
                new DiscordSeparatorComponent(),
                new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}")
            ];
            var container = new DiscordContainerComponent(comps, false, DiscordColor.Blurple);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(1, 51, 105))
                .WithTitle($"{titleEmoji} NFL Scores\r\n{displayName}")
                .WithDescription("");
            return embed;
        }
        #endregion

        #region CREATE STANDINGS EMBED BY CONFERENCE
        public async Task<List<DiscordEmbed>> CreateStandingsEmbedsByConferenceAsync()
        {
            var standings = await gameData.GetNFLStandingsAsync();
            var embeds = new List<DiscordEmbed>();
            var titleEmoji = NflEmojiService.GetEmoji("NFL");
            foreach (var conf in standings.Children)
            {
                var conferenceEmoji = conf.Name.Contains("American") ? NflEmojiService.GetEmoji("AFC") : NflEmojiService.GetEmoji("NFC");
                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{titleEmoji} NFL Standings\r\n{conferenceEmoji} {conf.Name}")
                    .WithColor(conf.Name.Contains("American") ? DiscordColor.Red : DiscordColor.Blue)
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter("Data from ESPN");

                var sortedEntries = conf.Standings.Entries
                    .OrderByDescending(e => e.Stats.Find(s => s.Name == "wins")?.Value ?? 0)
                    .ThenByDescending(e => e.Stats.Find(s => s.Name == "winPercent")?.Value ?? 0);

                var standingsLines = string.Join("\n", sortedEntries.Select(entry =>
                {
                    var stats = entry.Stats;
                    var wins = (int)(stats.Find(s => s.Name == "wins")?.Value ?? 0);
                    var losses = (int)(stats.Find(s => s.Name == "losses")?.Value ?? 0);
                    var ties = (int)(stats.Find(s => s.Name == "ties")?.Value ?? 0);
                    var winPercent = stats.Find(s => s.Name == "winPercent")?.DisplayValue ?? ".000";

                    var teamName = entry.Team.DisplayName.Length > 22
                    ? entry.Team.DisplayName[..22]
                    : entry.Team.DisplayName;

                    var record = ties > 0 ? $"{wins}-{losses}-{ties}" : $"{wins}-{losses}";
                    var winPercentPadded = $"({winPercent})"; 
                    return $"{teamName, -22} {record, 7} {winPercentPadded, 6}";
                }));

                var standingsText = "```\n" + string.Join("\n", standingsLines) + "\n```";
                embed.WithDescription(standingsText);
                embeds.Add(embed.Build());
            }

            return embeds;
        }
        #endregion

        #region CREATE STANDINGS EMBED
        public async Task<DiscordEmbed> CreateStandingsEmbedAsync(string? conference = null)
        {
            var standings = await gameData.GetNFLStandingsAsync();
            var titleEmoji = NflEmojiService.GetEmoji("NFL");
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{titleEmoji} NFL Standings")
                .WithColor(DiscordColor.Green)
                .WithTimestamp(DateTime.UtcNow)
                .WithFooter("Data from ESPN");

            var conferencesToDisplay = standings.Children;

            // Filter by conference if specified
            if (!string.IsNullOrEmpty(conference))
            {
                conferencesToDisplay = [.. standings.Children.Where(c => c.Name.Contains(conference, StringComparison.OrdinalIgnoreCase))];
            }

            foreach (var conf in conferencesToDisplay)
            {
                var sortedEntries = conf.Standings.Entries
                    .OrderByDescending(e => e.Stats.Find(s => s.Name == "wins")?.Value ?? 0)
                    .ThenByDescending(e => e.Stats.Find(s => s.Name == "winPercent")?.Value ?? 0);

                var standingsLines = string.Join("\n", sortedEntries.Select(entry =>
                {
                    var stats = entry.Stats;
                    var wins = (int)(stats.Find(s => s.Name == "wins")?.Value ?? 0);
                    var losses = (int)(stats.Find(s => s.Name == "losses")?.Value ?? 0);
                    var ties = (int)(stats.Find(s => s.Name == "ties")?.Value ?? 0);
                    var winPercent = stats.Find(s => s.Name == "winPercent")?.DisplayValue ?? ".000";

                    var teamName = entry.Team.DisplayName.Length > 22
                    ? entry.Team.DisplayName[..22]
                    : entry.Team.DisplayName;

                    var record = ties > 0 ? $"{wins}-{losses}-{ties}" : $"{wins}-{losses}";
                    return $"**{teamName, -22}** {record, 7} ({winPercent, 6})";
                }));

                var standingsText = "```\n" + string.Join("\n", standingsLines) + "\n```";
                embed.AddField(conf.Name, standingsText, inline: false);
            }

            return embed.Build();
        }
        #endregion

        #region CREATE TEAM STATS EMBED
        public async Task<DiscordMessageBuilder> CreateTeamStatsEmbed(NflTeamStatisticsResponse teamStats, NFLSeasonType seasonType, int seasonYear, string teamAbbr)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var titleEmoji = NflEmojiService.GetEmoji("NFL");
            var teamEmoji = NflEmojiService.GetEmoji(teamAbbr);
            var embeds = new DiscordEmbed[teamStats.Splits.Categories.Count];
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(0, 102, 204))
                .WithTitle($"")
                .WithFooter($"Gameday Tracker ")
                .WithTimestamp(DateTimeOffset.UtcNow);

            var sb = new StringBuilder();

            foreach (var split in teamStats.Splits.Categories)
            { 
                var statName  = split.Stats[0].DisplayName;
                var statValue = split.Stats[0].DisplayValue;
                var rank      = split.Stats[0].RankDisplayValue;
                sb.AppendLine($"**{statName}**: {statValue} - {rank}");
            }
           

            DiscordComponent[] comps =
            [
                new DiscordTextDisplayComponent($"{titleEmoji} {seasonYear} {seasonType} {teamEmoji} Statistics"),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent(sb.ToString()),
                new DiscordSeparatorComponent(),
                new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}")
               
            ];

            var container = new DiscordContainerComponent(comps, false, DiscordColor.Orange);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);
            return msg;
        }
        #endregion

        #region CREATE TEAM STATS PAGE
        public async Task<DiscordMessageBuilder> CreateTeamStatsPage(
            NflTeamStatisticsResponse teamStats,
            string emoji,
            NFLSeasonType seasonType,
            int seasonYear,
            int pageIndex)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var titleEmoji = NflEmojiService.GetEmoji("NFL");

            var category = teamStats.Splits.Categories[pageIndex];

            var components = new List<DiscordComponent>
            {
                new DiscordTextDisplayComponent($"{titleEmoji} {seasonYear} {seasonType} {emoji} Statistics {emoji}"),
                new DiscordSeparatorComponent(),
                new DiscordTextDisplayComponent($"**{category.DisplayName}**"),
                new DiscordSeparatorComponent()
            };

            var statsToShow = category.Stats.Take(10);
            // Add all stats from this category
            foreach (var stat in statsToShow)
            {
                components.Add(new DiscordTextDisplayComponent(
                    $"**{stat.DisplayName}**: {stat.Value} | Rank: {stat.RankDisplayValue}"));
            }

            components.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));
            components.Add(new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}"));
            components.Add(new DiscordSeparatorComponent());

            var container = new DiscordContainerComponent([.. components], false, DiscordColor.Orange);
            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(container);

            return msg;
        }
        #endregion

        #region CREATE SCOREBOARD PAGE
        public async Task<DiscordMessageBuilder> CreateScoreboardPage(NFLScoreboard scores, string emoji, NFLSeasonType seasonType, int seasonYear, int pageIndex)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var titleEmoji = NflEmojiService.GetEmoji("NFL");
            var displayName = gameData.GetFullSeasonWeekDisplay(scores);
            var eventList = new List<List<Event>>();

            var liveGames = gameData.GetLiveGames(scores);
            var completedGames = gameData.GetCompletedGames(scores);
            var scheduledGames = gameData.GetScheduledGames(scores);
            eventList.Add(liveGames);
            eventList.Add(completedGames);
            eventList.Add(scheduledGames);

            var gamesToShow = completedGames
                     .Skip(pageIndex * 4)
                     .Take(4)
                     .ToList();


            var components = new List<DiscordComponent>
            {
               new DiscordTextDisplayComponent($"{titleEmoji} **{displayName} Scores** {emoji}"),
               new DiscordSeparatorComponent(true),
            };

            foreach (var game in gamesToShow)
            {
                var gameInfo = await gameData.GetGameInfo(game);
                var headline = game.Competitions[0].Headlines?.FirstOrDefault()?.ShortLinkText ?? "not found";
                var seperators = new[] { "@", "VS" };
                var names = game.ShortName.Split(seperators, StringSplitOptions.TrimEntries);
                var awayEmoji = NflEmojiService.GetEmoji(names[0].Trim());
                var homeEmoji = NflEmojiService.GetEmoji(names[1].Trim());
                components.Add(new DiscordTextDisplayComponent($"{gameInfo}"));
                components.Add(new DiscordTextDisplayComponent($"📰 Headline {headline}\r\n"));
            }

            components.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));
            components.Add(new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}"));
            components.Add(new DiscordSeparatorComponent());

            var msg = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(new DiscordContainerComponent(components, false, DiscordColor.Blurple));
                return msg;

        }
        #endregion

        #region CREATE BETTING EMBED
        public async Task<DiscordMessageBuilder> BuildBettingEmbed(string data, string amount)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var away = data.Split(" at ")[0].Trim();
            var home = data.Split(" at ")[1].Trim();
            var components = new List<DiscordComponent>
            {
               new DiscordTextDisplayComponent($"Betting"),
               new DiscordSeparatorComponent(true),
               new DiscordTextDisplayComponent($"Place your bet on {away} or {home} with amount of {amount}"),
               new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
               new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}"),
            };

            var msg = new DiscordMessageBuilder()
               .EnableV2Components()
               .AddContainerComponent(new DiscordContainerComponent(components, false, DiscordColor.Green));
            return msg;
        }
        #endregion

        #region CREATE BETTING RESULT EMBED
        public async Task<DiscordMessageBuilder> BuildBettingResultEmbed(string data, string amount, string userName)
        {
            var timestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var away = data.Split(" at ")[0].Trim();
            var home = data.Split(" at ")[1].Trim();
            var components = new List<DiscordComponent>
            {
               new DiscordTextDisplayComponent($"Betting"),
               new DiscordSeparatorComponent(true),
               new DiscordTextDisplayComponent($"Place your bet on {away} or {home} with amount of {amount}"),
               new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
               new DiscordTextDisplayComponent($"Gameday Tracker {timestamp}"),
            };

            var msg = new DiscordMessageBuilder()
               .EnableV2Components()
               .AddContainerComponent(new DiscordContainerComponent(components, false, DiscordColor.Green));
            return msg;
        }
        #endregion

    }
}
