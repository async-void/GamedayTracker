using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Models.NFL;
using GamedayTracker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DiscordEmbed CreateScoresEmbed(NFLScoreboard data)
        {
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            //var seasonName = gameData.GetSeasonTypeName(data.Season.Type);
            var displayName = gameData.GetFullSeasonWeekDisplay(data);
            var embed = new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(1, 51, 105))
                .WithTitle($"🏈 NFL Scores\r\n{displayName}")
                .WithFooter("Gameday Tracker")
                .WithTimestamp(DateTimeOffset.UtcNow);

            if (data.Events == null || data.Events.Count == 0)
            {
                embed.WithDescription("No games scheduled for this week.");
                return embed.Build();
            }

            // Separate games by status
            var liveGames = gameData.GetLiveGames(data);
            var completedGames = gameData.GetCompletedGames(data);
            var scheduledGames = gameData.GetScheduledGames(data);

            // Add live games first
            if (liveGames.Count > 0)
            {
                embed.AddField("🔴 LIVE GAMES", "---");
                foreach (var game in liveGames)
                {
                    var gameInfo = gameData.GetGameInfo(game).Result;
                    var emoji = NflEmojiService.GetEmoji(game.ShortName);
                    embed.AddField(emoji, gameInfo, inline: false);
                }
            }

            // Add completed games
            if (completedGames.Count > 0)
            {
                embed.AddField("✅ COMPLETED", "---");
                foreach (var game in completedGames)
                {
                    var gameInfo = gameData.GetGameInfo(game).Result;
                    var seperators = new[] { "@", "VS" };
                    var names = game.ShortName.Split(seperators, StringSplitOptions.TrimEntries);
                    var awayEmoji = NflEmojiService.GetEmoji(names[0].Trim());
                    var homeEmoji = NflEmojiService.GetEmoji(names[1].Trim());
                    embed.AddField($"{awayEmoji} at {homeEmoji}", gameInfo, inline: false);
                }
            }

            // Add scheduled games
            if (scheduledGames.Count > 0)
            {
                embed.AddField("📅 SCHEDULED", "---");
                foreach (var game in scheduledGames)
                {
                    var gameInfo = gameData.GetGameInfo(game).Result;
                    var seperators = new[] { "@", "VS" };
                    var names = game.ShortName.Split(seperators, StringSplitOptions.TrimEntries);
                    var awayEmoji = NflEmojiService.GetEmoji(names[0].Trim());
                    var homeEmoji = NflEmojiService.GetEmoji(names[1].Trim());
                    embed.AddField($"{awayEmoji} at {homeEmoji}", gameInfo, inline: false);
                }
            }

            return embed.Build();
        }
        #endregion

        #region CREATE STANDINGS EMBED BY CONFERENCE
        public async Task<List<DiscordEmbed>> CreateStandingsEmbedsByConferenceAsync()
        {
            var standings = await gameData.GetNFLStandingsAsync();
            var embeds = new List<DiscordEmbed>();

            foreach (var conf in standings.Children)
            {
                var conferenceEmoji = conf.Name.Contains("American") ? NflEmojiService.GetEmoji("AFC") : NflEmojiService.GetEmoji("NFC");
                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{conferenceEmoji} {conf.Name}")
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
                    ? entry.Team.DisplayName.Substring(0, 22)
                    : entry.Team.DisplayName;

                    var record = ties > 0 ? $"{wins}-{losses}-{ties}" : $"{wins}-{losses}";
                    return $"{teamName, -22} {record, 7} ({winPercent, 6})";
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
            var embed = new DiscordEmbedBuilder()
                .WithTitle("🏈 NFL Standings")
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
                    ? entry.Team.DisplayName.Substring(0, 22)
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
    }
}
