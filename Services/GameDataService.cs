﻿using GamedayTracker.Enums;
using GamedayTracker.Extensions;
using GamedayTracker.Factories;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ChalkDotNET;

namespace GamedayTracker.Services
{
    public class GameDataService : IGameData
    {
        private readonly AppDbContextFactory _dbFactory = new AppDbContextFactory();
        
        /// <summary>
        /// Get Scoreboard for season and week
        /// </summary>
        /// <param name="season"></param>
        /// <param name="week"></param>
        /// <returns>List Matchup</returns>
        #region GET SCOREBOARD
        public Result<List<Matchup>, SystemError<GameDataService>> GetScoreboard(int season, int week)
        {
            var matchups = new List<Matchup>();
            var db = _dbFactory.CreateDbContext();

            var weeklyMatchups = db.Matchups?
                .Where(x => x.Season == season && x.Week == week)
                .Include(x => x.Opponents)
                .Include(x => x.Opponents.AwayTeam)
                .Include(x => x.Opponents.HomeTeam)
            .ToList();

            if (weeklyMatchups!.Count > 0)
            {
                return Result<List<Matchup>, SystemError<GameDataService>>.Ok(weeklyMatchups);
            }

            Console.WriteLine($"no matchups in database for Season {season} : Week {week}\r\nAttempting to collect data from website!");
            var seasonType = "reg";

            var link = $"https://www.footballdb.com/scores/index.html?lg=NFL&yr={season}&type={seasonType}&wk={week}";
            var web = new HtmlWeb();
            var doc = web.Load(link);

            var scoreboardNodes = doc.DocumentNode.SelectNodes(".//div[@class='lngame']//table");

            if (scoreboardNodes is not null)
            {
                for (var i = 0; i <= scoreboardNodes.Count - 1; i++)
                {
                    var node = scoreboardNodes[i].ChildNodes[3];
                    if (node.HasChildNodes)
                    {
                        try
                        {
                            var matchup = ParseMatchup(node, season, week);

                            if (matchup.IsOk)
                                matchups.Add(matchup.Value);
                            else
                            {
                                Console.WriteLine($"{Chalk.Red("[ERROR]")} {Chalk.DarkGray(matchup.Error.ErrorMessage!)}");
                            }
                        }
                        catch (Exception e)
                        {
                            var error = new SystemError<GameDataService>()
                            {
                                ErrorMessage = e.Message,
                                CreatedBy = this,
                                CreatedAt = DateTime.UtcNow,
                                ErrorType = Enums.ErrorType.INFORMATION,
                            };
                            Console.WriteLine($"{Chalk.Red("[ERROR]")} {Chalk.DarkGray(error.ErrorMessage)}");
                        }
                    }
                }
                Console.WriteLine($"{Chalk.Green("Week [")}{Chalk.Yellow($"{week}")}{Chalk.Green("] Complete...")}");

                //db.AddRange(matchups);
                //db.SaveChanges();

                return Result<List<Matchup>, SystemError<GameDataService>>.Ok(matchups);
            }

            return Result<List<Matchup>, SystemError<GameDataService>>.Err(new SystemError<GameDataService>
            {
                ErrorMessage = "no matchups found!",
                ErrorType = ErrorType.WARNING,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }
        #endregion

        /// <summary>
        /// Parse Matchup Node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="season"></param>
        /// <param name="week"></param>
        /// <returns>Matchup</returns>
        #region PARSE MATCHUP

        public Result<Matchup, SystemError<GameDataService>> ParseMatchup(HtmlNode node, int season, int week)
        {
            try
            {
                var awayNode = node.ChildNodes[1];
                var awayName = awayNode.ChildNodes[1].InnerText;
                var homeNode = node.ChildNodes[3];
                var homeName = homeNode.ChildNodes[1].InnerText;
                var awayMatch = Regex.Match(awayName, """(?<name>[\w\s]+) (?<rec>\(\d+-\d+\))""");
                var homeMatch = Regex.Match(homeName, """(?<name>[\w\s]+) (?<rec>\(\d+-\d+\))""");

                var awayNameFinal = awayMatch.Groups["name"];
                var awayAbbr = awayNameFinal.Value.ToAbbr();
                var awayRecord = awayMatch.Groups["rec"];

                var homeNameFinal = homeMatch.Groups["name"];
                var homeAbbr = homeNameFinal.Value.ToAbbr();
                var homeRecord = homeMatch.Groups["rec"];

                var awayScore = awayNode.LastChild.InnerText;
                var homeScore = homeNode.LastChild.InnerText;

                var awayTeam = new Team()
                {
                    Name = awayNameFinal.Value,
                    Score = int.Parse(awayScore),
                    Record = awayRecord.Value,
                    Division = awayNameFinal.Value.ToDivision(),
                    Abbreviation = awayAbbr,
                    LogoPath = LogoPathService.GetLogoPath(awayAbbr),
                    Emoji = NflEmojiService.GetEmoji(awayAbbr)
                };

                var homeTeam = new Team()
                {
                    Name = homeNameFinal.Value,
                    Score = int.Parse(homeScore),
                    Record = homeRecord.Value,
                    Division = homeNameFinal.Value.ToDivision(),
                    Abbreviation = homeAbbr,
                    LogoPath = LogoPathService.GetLogoPath(homeAbbr),
                    Emoji = NflEmojiService.GetEmoji(homeAbbr)
                };
                var matchup = new Matchup()
                {
                    Week = week,
                    Season = season,
                    Opponents = new Opponent { AwayTeam = awayTeam, HomeTeam = homeTeam }
                };


                return Result<Matchup, SystemError<GameDataService>>.Ok(matchup);
            }
            catch (Exception)
            {
                var error = new SystemError<GameDataService>
                {
                    ErrorType = ErrorType.INFORMATION,
                    ErrorMessage = "An Error occured while parsing the matchup data!",
                    CreatedBy = this,
                    CreatedAt = DateTime.UtcNow,
                };
                return Result<Matchup, SystemError<GameDataService>>.Err(error);
            }
        }

        #endregion
    }
}
