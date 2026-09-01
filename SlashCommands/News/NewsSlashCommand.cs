using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Schedules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;

namespace GamedayTracker.SlashCommands.News
{
    [Command("news")]
    [Description("Commands related to NFL News and Updates.")]
    public class NewsSlashCommand(INewsService newsService, ISchedulerFactory schedulerFactory, ILogger<NewsSlashCommand> logger)
    {
        
        #region GET NEWS HEADLINES
        [Command("get")]
        [Description("Gets the most recent NFL News and Updates.")]
        public async Task GetNewOrUpdates(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var rnd = new Random();
            var articles = await newsService.GetNews();
            var imgList = new List<string>();
            if (articles.IsOk)
            {
                var sBuilder = new StringBuilder();
                var count = articles.Value.Count;
                var embedTitle = $"**Latest NFL News {DateTime.UtcNow.ToLongDateString()}**";
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
                    new DiscordSectionComponent( new DiscordTextDisplayComponent($"-# Powered by Gameday Tracker ©️ <t:{unixTimestamp}:F>"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "donateId", "Donate"))
                   
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.Gold);
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
                logger.LogInformation("NFL News fetched and sent successfully.");
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            else
            {
                var errorMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Error:")
                        .WithDescription($"An error occurred while fetching the news. Please try again later.\r\n{articles.Error.ErrorMessage}")
                        .AddField("CreatedBy", articles!.Error!.CreatedBy!.ToString()!, true)
                        .AddField("CreatedAt", articles!.Error!.CreatedAt!.ToString()!, true));
                await ctx.RespondAsync(new DiscordWebhookBuilder(errorMessage));

            }
        }
        #endregion

        #region ENABLE DAILY HEADLINES
        [Command("enable-daily")]
        [Description("Sets the daily news headlines, default interval = 24h")]
        [RequirePermissions(DiscordPermission.ManageGuild)]
        public async Task EnableDailyHeadlines(CommandContext ctx, [Description("Interval in hours")] int interval = 24)
        {
            await ctx.DeferResponseAsync();

            var scheduler = await schedulerFactory.GetScheduler();
            var jobs = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            var jobExists = jobs.Any(j => j.Name.Equals("DailyHeadlineJob") && j.Group.Equals("NFL News"));
            if (jobExists)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("A daily news headline job is already scheduled. Please remove it before setting a new one."));
                return;
            }

            var dailyScheduler = ctx.ServiceProvider.GetService<DailyHeadlinesScheduler>();
            await dailyScheduler!.StartAsync();
            var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (interval != 24)
            {
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent("### ❌ Invalid Interval ❌"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent("The default interval for setting daily news headlines is 24 hours. The developers are working hard to implement user-defined intervals in the future!"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"GamedayTracker ©️ <t:{unixTimestamp}:F>" ),
                        new DiscordButtonComponent(DiscordButtonStyle.Success, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Red);
                await ctx.RespondAsync(new DiscordMessageBuilder().EnableV2Components().AddContainerComponent(container));
            }
            else
            {
                //TODO: Implement the logic to set daily news headline job with the specified interval.
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent("### ✔️ Daily Headlines Schedule Success ✔️"),
                    new DiscordSeparatorComponent(true),
                    new DiscordTextDisplayComponent("The default interval for setting daily news headlines is 24 hours. The developers are working hard to implement user-defined intervals in the future!"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"-# GamedayTracker ©️ <t:{unixTimestamp}:F>" ),
                        new DiscordButtonComponent(DiscordButtonStyle.Success, "donateId", "Donate"))
                ];
                var container = new DiscordContainerComponent(components, false, DiscordColor.Blurple);
                await ctx.RespondAsync(new DiscordMessageBuilder().EnableV2Components().AddContainerComponent(container));
            }
        }
        #endregion
       
    }
}
