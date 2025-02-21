using GamedayTracker.Enums;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.Services
{
    public class TeamDataService : ITeamData
    {
        public async Task<Result<List<DraftEntity>, SystemError<TeamDataService>>> GetDraftResultsAsync(int year)
        {
            await using var db = new AppDbContextFactory().CreateDbContext();
            var entityList = new List<DraftEntity>();
            var dbList = await db.DraftEntities.Where(x => x.Season == year).ToListAsync();

            if (dbList.Count > 0) 
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(dbList);

            for (var i = 1; i < 8; i++)
            {
                var link = $"https://www.footballdb.com/draft/draft.html?lg=NFL&yr={year}&rnd={i}";
                var web = new HtmlWeb();
                var doc = web.Load(link);

                var nodes = doc.DocumentNode.SelectNodes(".//table[contains(@class, 'statistics')]//tbody//tr");
                if (nodes is null) continue;
                var nodeCount = nodes.Count;
                for (var j = 0; j < nodeCount; j++)
                {
                    var curNode = nodes[j];
                    if (!curNode.HasChildNodes) continue;
                    if (curNode.ChildNodes.Count != 7) continue;

                    var round = curNode.ChildNodes[0].InnerText.Split(" ")[0];
                    var pick = curNode.ChildNodes[1].InnerText;
                    var teamName = curNode.ChildNodes[2].ChildNodes[0].ChildNodes[0].InnerText;
                    var playerName = curNode.ChildNodes[3].InnerText;
                    var pos = curNode.ChildNodes[4].InnerText;
                    var college = curNode.ChildNodes[5].InnerText;

                    var de = new DraftEntity()
                    {
                        Season = year,
                        College = college,
                        PickPosition = pick,
                        PlayerName = playerName,
                        Pos = pos,
                        Round = round,
                        TeamName = teamName
                    };
                    entityList.Add(de);
                }
            }

            if (entityList.Count == 0)
            {
                return Result<List<DraftEntity>, SystemError<TeamDataService>>.Err(new SystemError<TeamDataService>()
                {
                    ErrorMessage = "entity list was empty.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            db.DraftEntities.AddRange(entityList);
            await db.SaveChangesAsync();
            return Result<List<DraftEntity>, SystemError<TeamDataService>>.Ok(entityList);
        }
    }
}
