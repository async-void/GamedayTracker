using GamedayTracker.Models;
using GamedayTracker.Models.News;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface INewsService
    {
        Task<Result<List<NewsArticle>, SystemError<NFLNewsService>>> GetNews();
        Task<Result<List<NewsArticle>, SystemError<NFLNewsService>>> GetNewsForTeam(string teamAbbr);
    }
}
