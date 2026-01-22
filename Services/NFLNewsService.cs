using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Models.News;
using HtmlAgilityPack;
using System.Net.Http.Json;

namespace GamedayTracker.Services
{
    public class NFLNewsService : INewsService
    {
        #region GET NFL NEWS

        public async Task<Result<List<NewsArticle>, SystemError<NFLNewsService>>> GetNews()
        {
            string ESPN_NEWS_URL = "https://site.api.espn.com/apis/site/v2/sports/football/nfl/news?limit=5";
            var news = new List<NewsArticle>();
            using var client = new HttpClient();


            try
            {

                var response = await client.GetFromJsonAsync<EspnNewsResponse>(ESPN_NEWS_URL);

                Console.WriteLine("Latest NFL Headlines:\n");

                foreach (var article in response.Articles)
                {
                    var _article = new NewsArticle
                    {
                        Headline = article.Headline,
                        Description = article.Description,
                        Published = article.Published,
                        Images = article.Images,
                        Links = new ArticleLinks
                        {
                            Web = article.Links.Web
                        }
                    };
                    news.Add(_article);
                }
                if (news.Count == 0)
                {
                    return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Err(new SystemError<NFLNewsService>
                    {
                        ErrorCode = Guid.NewGuid(),
                        ErrorMessage = "No news articles found.",
                        CreatedAt = DateTimeOffset.UtcNow,
                        ErrorType = ErrorType.INFORMATION,
                    });
                }
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Ok(news);
            }
            catch (Exception e)
            {
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Err(new SystemError<NFLNewsService>
                {
                    ErrorCode = Guid.NewGuid(),
                    ErrorMessage = $"No news articles found. {e.Message}",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                });
            }

        }
        #endregion

        #region GET NEWS FOR SPECIFIC TEAM
        public async Task<Result<List<NewsArticle>, SystemError<NFLNewsService>>> GetNewsForTeam(string teamAbbr)
        {
            string ESPN_NEWS_URL = $"https://site.api.espn.com/apis/site/v2/sports/football/nfl/news?limit=5&team={teamAbbr}";
            var news = new List<NewsArticle>();
            using var client = new HttpClient();

            try
            {
                var response = await client.GetFromJsonAsync<EspnNewsResponse>(ESPN_NEWS_URL);

                foreach (var article in response.Articles)
                {

                    var _article = new NewsArticle
                    {
                        Headline = article.Headline,
                        Description = article.Description,
                        Published = article.Published,
                        Images = article.Images,
                        Links = new ArticleLinks
                        {
                            Web = article.Links.Web
                        }
                    };
                    news.Add(_article);
                }
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Ok(news);
            }
            catch(Exception e)
            {
                return Result<List<NewsArticle>, SystemError<NFLNewsService>>.Err(new SystemError<NFLNewsService>
                {
                    ErrorCode = Guid.NewGuid(),
                    ErrorMessage = $"No news articles found for team {teamAbbr}. {e.Message}",
                    CreatedAt = DateTimeOffset.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                });
            }
        }
        #endregion
    }
}
