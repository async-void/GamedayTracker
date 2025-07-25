using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using Quartz;
using Serilog;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class DailyHeadlineJob(INewsService newService, IJsonDataService dataService, DiscordClient client) : IJob
    {
        private readonly INewsService _newsService = newService;
        private readonly IJsonDataService _dataService = dataService;
        private readonly DiscordClient _client = client;
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("Executing Daily Headline Job...");
            //var guilds = await _dataService.GetGuildsFromJsonAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var rnd = new Random();
            var articles = _newsService.GetNews();
            var imgList = new List<string>();
            if (articles.IsOk)
            {
                var sBuilder = new StringBuilder();
                var count = articles.Value.Count;
                var embedTitle = $"**Latest NFL News {DateTime.UtcNow.ToLongDateString()}**";

                for (var i = 0; i < count; i++)
                {
                    sBuilder.AppendLine($"{i + 1}. **{articles.Value[i].Title}**\r\n{articles.Value[i].Content}");
                    imgList.Add(articles.Value[i].ImgUrl!);
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"<:newspaper:1331763576150425620> {embedTitle}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{sBuilder}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem(imgList[rnd.Next(0, imgList.Count)], "news", false)),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent( new DiscordTextDisplayComponent($"Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))

                ];

                var container = new DiscordContainerComponent(components);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                #region Depracated Code - I will remove this code soon as I see the new code working.
                //if (guilds.IsOk && guilds.Value.Count > 0)
                //{
                //    foreach (var g in guilds.Value)
                //    {
                //        if (g.IsDailyHeadlinesEnabled)
                //        {
                //            if (g.NotificationChannelId is null)
                //            {
                //                Log.Warning($"Guild {g.GuildId} has no notification channel set for daily headlines.");
                //                continue;
                //            }
                //            var chnl = await _client.GetChannelAsync(ulong.Parse(g.NotificationChannelId));
                //            if (chnl is { } ch)
                //            {
                //                await ch.SendMessageAsync(message);
                //                await Task.Delay(200); 
                //            }
                //        }
                //    }
                //}
                #endregion

                var chnl = await _client.GetChannelAsync(1398021268032196698);
                var msg = await chnl.SendMessageAsync(message);
                await chnl.CrosspostMessageAsync(msg);
            }
            else
                Log.Error("Failed to fetch news articles.");

        }
    }
}
