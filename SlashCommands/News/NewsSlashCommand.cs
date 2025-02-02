using System.ComponentModel;
using System.Text;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands.News
{
    public class NewsSlashCommand
    {
        private readonly NFLNewsService _newsService = new();

        [DSharpPlus.Commands.Command("news")]
        [Description("Gets the most recent NFL News and Updates.")]
        public async Task GetNewOrUpdates(CommandContext ctx)
        {
            var rnd = new Random();
            var articles = _newsService.GetNews();
            var imgList = new List<string>();
            if (articles.IsOk)
            {
                var sBuilder = new StringBuilder();
                var count = articles.Value.Count;
                var embedTitle = $"**Latest NFL News {DateTime.UtcNow}**";
                sBuilder.Append($"<:newspaper:1331763576150425620> {embedTitle}\r\n\r\n");

                for (int i = 0; i < count; i++)
                {
                    var title = articles.Value[i].Title;
                    var content = articles.Value[i].Content;
                    imgList.Add(articles!.Value[i]!.ImgUrl!);

                    sBuilder.Append($"**{i + 1}) {title}**\r\n{content}\r\n\r\n");
                }

                imgList.Shuffle();
                await ctx.DeferResponseAsync();
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithDescription(sBuilder.ToString())
                        .WithImageUrl(imgList[rnd.Next(1, 3)])
                        .WithTimestamp(DateTime.UtcNow));
               
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
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
