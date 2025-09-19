using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using HtmlAgilityPack;

namespace GamedayTracker.Services
{
    public class NFLNewsService : INewsService
    {
        #region GET NFL NEWS

        public Result<List<NewsArticle>, SystemError<NFLNewsService>> GetNews()
        {
            const string baseLink = "https://www.espn.com/nfl/";
            var web = new HtmlWeb();
            var doc = web.Load(baseLink);
            var newsArticles = new List<NewsArticle>();

            var articles = doc.DocumentNode.SelectNodes("//article[contains(@class, 'editorial')]//ul");

            if (articles is null || !articles.Any())
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Err(new SystemError<NFLNewsService>
                {
                    ErrorMessage = "No articles found.",
                    ErrorType = ErrorType.WARNING,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = this
                });

            for (int i = 1; i < articles.Count - 1; i++)
            {
                var curArticle = articles[i];
                if (curArticle.HasChildNodes)
                {
                    var childCount = curArticle.ChildNodes.Count;
                    for (int j = 0; j < childCount; j += 2)
                    {
                        //even child nodes has image data
                        //odd child nodes have title and content

                        var cNode = curArticle.ChildNodes[j + 1];

                        var imgNode = curArticle.ChildNodes[j].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0];
                        var title = "";
                        var titleNode = cNode.ChildNodes[0];
                        if (titleNode.InnerText != "")
                            title = titleNode.InnerText;

                        var content = "";
                        var contentNode = cNode.ChildNodes[1];
                        if (contentNode.InnerText != "")
                            content = contentNode.InnerText;

                        var imgUrlNode = imgNode.ChildNodes[2].Attributes["data-default-src"];
                        var imgUrl = "";
                        if (imgUrlNode.Value != "")
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
            }
            
            if (newsArticles is { Count: > 0 }) 
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Ok(newsArticles);

            //if the newsArticles are empty or null return Error
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
