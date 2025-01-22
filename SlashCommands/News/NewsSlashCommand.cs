using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GamedayTracker.Services;
using GamedayTracker.Utility;

namespace GamedayTracker.SlashCommands.News
{
    public class NewsSlashCommand: ApplicationCommandModule
    {
        private readonly NFLNewsService _newsService = new();

        [SlashCommand("news", "Gets the most recent NFL News and Updates.")]
        public async Task GetNewOrUpdates(InteractionContext ctx)
        {
            var rnd = new Random();
            var articles = _newsService.GetNews();
            var imgList = new List<string>();
            if (articles.IsOk)
            {
                var sBuilder = new StringBuilder();
                var count = articles.Value.Count;
                var embedTitle = $"**Latest NFL News {DateTime.UtcNow}**";
                sBuilder.Append($"{embedTitle}\r\n\r\n");

                for (int i = 0; i < count; i++)
                {
                    var title = articles.Value[i].Title;
                    var content = articles.Value[i].Content;
                    imgList.Add(articles!.Value[i]!.ImgUrl!);

                    sBuilder.Append($"**{i + 1}) {title}**\r\n{content}\r\n\r\n");
                }

                imgList.Shuffle();
                await ctx.DeferAsync();
                var message = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithDescription(sBuilder.ToString())
                        .WithImageUrl(imgList[rnd.Next(1, 3)])
                        .WithTimestamp(DateTime.UtcNow));
               
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(message));
            }
            else
            {
                await ctx.DeferAsync();
                var errorMessage = new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle($"Error: {articles.Error.ErrorMessage}")
                        .AddField("CreatedBy", articles!.Error!.CreatedBy!.ToString(), true)
                        .AddField("CreatedAt", articles!.Error!.CreatedAt!.ToString(), true));
                await ctx.EditResponseAsync(new DiscordWebhookBuilder(errorMessage));

            }
        }
    }
}
