using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Interfaces;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands.News
{
    public class NewsSlashCommand(INewsService newsService)
    {
        
        [Command("news")]
        [Description("Gets the most recent NFL News and Updates.")]
        public async Task GetNewOrUpdates(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var rnd = new Random();
            var articles = newsService.GetNews();
            var imgList = new List<string>();
            if (articles.IsOk)
            {
                var sBuilder = new StringBuilder();
                var count = articles.Value.Count;
                var embedTitle = $"**Latest NFL News {DateTime.UtcNow.ToLongDateString()}**";
                
                DiscordComponent[] components =
                [
                    new DiscordTextDisplayComponent($"<:newspaper:1331763576150425620> {embedTitle}"),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"1. **{articles.Value[0].Title}**\r\n{articles.Value[0].Content}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "art1Id", "Read More")),
                    new DiscordSeparatorComponent(true),
                  
                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"2. **{articles.Value[1].Title}**\r\n{articles.Value[1].Content}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "art1Id1", "Read More")),
                    new DiscordSeparatorComponent(true),

                    new DiscordSectionComponent(new DiscordTextDisplayComponent($"3. **{articles.Value[2].Title}**\r\n{articles.Value[2].Content}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "art1Id2", "Read More")),
                    new DiscordSeparatorComponent(true),

                    new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem(articles!.Value[rnd.Next(1, 3)]!.ImgUrl!, "news", false)),
                    new DiscordSeparatorComponent(true),
                    new DiscordSectionComponent( new DiscordTextDisplayComponent($"Gameday Tracker ©️ {DateTime.UtcNow.ToLongDateString()}"),
                        new DiscordButtonComponent(DiscordButtonStyle.Success, "donateId", "Donate"))
                   
                ];

                var container = new DiscordContainerComponent(components, false, DiscordColor.Gold);
                var message = new DiscordInteractionResponseBuilder()
                    .EnableV2Components()
                    .AddContainerComponent(container);
               
                await ctx.RespondAsync(new DiscordInteractionResponseBuilder(message));
            }
            else
            {
                await ctx.DeferResponseAsync();
                var errorMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Error: {articles.Error.ErrorMessage}")
                        .AddField("CreatedBy", articles!.Error!.CreatedBy!.ToString()!, true)
                        .AddField("CreatedAt", articles!.Error!.CreatedAt!.ToString()!, true));
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(errorMessage));

            }
        }
    }
}
