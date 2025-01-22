using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using GamedayTracker.Enums;
using GamedayTracker.Models;
using HtmlAgilityPack;

namespace GamedayTracker.Services
{
    public class NFLNewsService
    {
        #region GET NFL NEWS

        public Result<List<NewsArticle>, SystemError<NFLNewsService>> GetNews()
        {
            var baseLink = "https://www.espn.com/nfl";
            var web = new HtmlWeb();
            var doc = web.Load(baseLink);
            var newsArticles = new List<NewsArticle>();

            var articles = doc.DocumentNode.SelectNodes("//article[contains(@class, 'editorial')]//ul");

            if (articles is null || articles!.Count <= 0)
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Err(new SystemError<NFLNewsService>
                {
                    ErrorMessage = "no articles found.",
                    ErrorType = ErrorType.WARNING,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });

            var curArticle = articles[0];
            if (curArticle.HasChildNodes)
            {
                for (int i = 0; i < curArticle.ChildNodes.Count; i+= 2)
                {
                    //even child nodes has image data
                    //odd child nodes have title and content
                    var childCount = curArticle.ChildNodes.Count;
                    var cNode= curArticle.ChildNodes[i + 1];

                    var imgNode = curArticle.ChildNodes[i].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0];
                    var title = "";
                    var titleNode = cNode.ChildNodes[0];
                    if (titleNode is not null)
                        title = titleNode.InnerText;

                    var content = "";
                    var contentNode = cNode.ChildNodes[1];
                    if (contentNode is not null)
                        content = contentNode.InnerText;

                    var imgUrlNode = imgNode.ChildNodes[2].Attributes["data-default-src"];
                    var imgUrl = "";
                    if (imgUrlNode is not null)
                        imgUrl = imgUrlNode.Value;

                    var newsArticle = new NewsArticle
                    {
                        ImgUrl = imgUrl,
                        Title = title,
                        Content = content

                    };

                    newsArticles.Add(newsArticle);

                }
            }
            if (newsArticles.Count > 0) 
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Ok(newsArticles);
            return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Err(new SystemError<NFLNewsService>
            {
                ErrorMessage = "no news articles found.",
                ErrorType = ErrorType.INFORMATION,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }
        #endregion
    }
}
