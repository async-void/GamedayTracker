using DSharpPlus;
using DSharpPlus.Entities;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Utility;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using System.Text;

namespace GamedayTracker.Jobs
{
    public class DailyHeadlineJob(IEvaluator evaluator, INewsService newsService, DiscordClient client, ILogger<DailyHeadlineJob> logger) : IJob
    {
       
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Executing Daily Headline Job...");
            var seasonType = evaluator.Evaluate(DateTime.Now);

            if (seasonType == RealTimeScoresMode.Offseason)
            {
                logger.LogInformation("Offseason mode, skipping DailyHeadlineJob...");
                return;
            }
            var unixTimestamp = DateTimeOffset.UtcNow.ToTimestamp();
            var rnd = new Random();
            var articles = await newsService.GetNews();
            var imgList = new List<string>();
            if (articles.IsOk)
            {
                var sBuilder = new StringBuilder();
                var count = articles.Value.Count;
                var embedTitle = $"**Latest NFL News {DateTimeOffset.UtcNow:D}**";

                for (var i = 0; i < count; i++)
                {
                    sBuilder.AppendLine($"{i + 1}. **{articles.Value[i].Headline}**\r\n{articles.Value[i].Description}");

                    for (var j = 0; j < articles.Value[i].Images.Count; j++)
                    {
                        imgList.Add(articles.Value[i].Images[j].Url);
                    }
                }

                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"<:newspaper:1331763576150425620> {embedTitle}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent($"{sBuilder}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem(imgList[rnd.Next(0, imgList.Count)], "news", false)),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent( new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ {unixTimestamp}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))

                ];

                var container = new DiscordContainerComponent(components);
                var message = new DiscordMessageBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);

                var chnl = await client.GetChannelAsync(1398021268032196698);
                var msg = await chnl.SendMessageAsync(message);

                try
                {
                    await chnl.CrosspostMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    logger.LogInformation("unable to crosspost daily headlines - {}", ex.Message);
                }
                
            }
            else
                logger.LogError("Failed to fetch news articles.");

        }

        private async Task GetNFLHeadlines()
        {
            var headlineUrl = "https://";
        }
    }
}
