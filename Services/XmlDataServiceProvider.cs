using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using GamedayTracker.ChoiceProviders;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace GamedayTracker.Services
{
    public class XmlDataServiceProvider : IXmlDataService
    {
        #region WRITE MATCHUP DATA

        public Task<Result<bool, SystemError<XmlDataServiceProvider>>> WriteMatchupAsync(Matchup matchup)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region WRITE ALL MATCHUPS TO XML

        public async Task<Result<bool, SystemError<XmlDataServiceProvider>>> WriteAllMatchupsToXmlAsync(
            List<Matchup> matchups, string season)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML",
                $"game_data_{season}.xml");

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath) ??
                                          throw new InvalidOperationException(
                                              "Unable to create directory for XML file."));
                //create new XmlDocument
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                    new XElement("Games",
                        new XAttribute("Season", season),
                        matchups.Select(m =>
                            new XElement("Game",
                                new XAttribute("Season", season),
                                new XAttribute("Week", m.Week),
                                new XElement("Id", m.Id),
                                new XElement("GameTime", m.GameTime ?? ""),
                                new XElement("GameDate", m.GameDate ?? ""),
                                new XElement("Opponents",
                                    new XElement("Away_Team",
                                        new XAttribute("Name", m.Opponents.AwayTeam.Name),
                                        new XAttribute("Score", m.Opponents.AwayTeam.Score ?? 0),
                                        new XAttribute("Abbr", m.Opponents.AwayTeam.Abbreviation),
                                        new XAttribute("Division", m.Opponents.AwayTeam.Division),
                                        new XAttribute("Record", m.Opponents.AwayTeam.Record),
                                        new XAttribute("Logo_Path", m.Opponents.AwayTeam.LogoPath),
                                        new XAttribute("Emoji", m.Opponents.AwayTeam.Emoji)),
                                    new XElement("Home_Team",
                                        new XAttribute("Name", m.Opponents.HomeTeam.Name),
                                        new XAttribute("Score", m.Opponents.HomeTeam.Score ?? 0),
                                        new XAttribute("Abbr", m.Opponents.HomeTeam.Abbreviation),
                                        new XAttribute("Division", m.Opponents.HomeTeam.Division),
                                        new XAttribute("Record", m.Opponents.HomeTeam.Record),
                                        new XAttribute("Logo_Path", m.Opponents.HomeTeam.LogoPath),
                                        new XAttribute("Emoji", m.Opponents.HomeTeam.Emoji)))))));
                await using var stream = File.Create(filePath);
                await doc.SaveAsync(stream, SaveOptions.None, CancellationToken.None);

                return Result<bool, SystemError<XmlDataServiceProvider>>.Ok(true);

            }
            else
            {
                return Result<bool, SystemError<XmlDataServiceProvider>>.Err(new SystemError<XmlDataServiceProvider>()
                {
                    ErrorMessage = "Game data already exists, could not over write game data.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
        }

        #endregion

        #region WRITE ALL STANDINGS TO XML
        public async Task<Result<bool, SystemError<XmlDataServiceProvider>>> WriteSeasonStandingsToXmlAsync(List<TeamStanding> standings, int season)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"standings_data_{season}.xml");
            if (File.Exists(filePath))
            {
                return Result<bool, SystemError<XmlDataServiceProvider>>.Err(new SystemError<XmlDataServiceProvider>()
                {
                    ErrorMessage = "Standings data already exists, could not over write standings data.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            //create new XmlDocument
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                new XElement("Standings",
                    new XAttribute("Season", season),
                    standings.Select(s =>
                        new XElement("Team",
                            new XAttribute("Name", s.TeamName),
                            new XAttribute("Division", s.Division),
                            new XAttribute("Abbreviation", s.Abbr),
                            new XAttribute("Wins", s.Wins),
                            new XAttribute("Losses", s.Loses)))));
            await using var stream = File.Create(filePath);
            await doc.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
            return Result<bool, SystemError<XmlDataServiceProvider>>.Ok(true);

        }
        #endregion

        #region GET SEASON STANDINGS FROM XML
        public async Task<Result<List<TeamStanding>, SystemError<XmlDataServiceProvider>>> GetSeasonStandingsFromXmlAsync(int season)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"standings_data_{season}.xml");
            if (!File.Exists(filePath))
            {
                return Result<List<TeamStanding>, SystemError<XmlDataServiceProvider>>.Err(new SystemError<XmlDataServiceProvider>()
                {
                    ErrorMessage = "Standings data does not exist for the specified season.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }
            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);

            var standings = doc.Descendants("Team")
                .Select(t => new TeamStanding
                {
                    TeamName = t.Attribute("Name")?.Value ?? "",
                    Division = t.Attribute("Division")?.Value ?? "",
                    Abbr = t.Attribute("Abbreviation")?.Value ?? "",
                    Wins = t.Attribute("Wins")?.Value ?? "0",
                    Loses = t.Attribute("Losses")?.Value ?? "0",
                    Ties = t.Attribute("Ties")?.Value ?? "0",
                    Pct = t.Attribute("Pct")?.Value ?? "0.0",
                }).ToList();
            return Result<List<TeamStanding>, SystemError<XmlDataServiceProvider>>.Ok(standings);
        }
        #endregion

        #region GET SEASON MATCHUP DATA

        public async Task<Result<List<Matchup>, SystemError<XmlDataServiceProvider>>> GetSeasonMatchupDataAsync(string season)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"game_data_{season}.xml");
            if (!File.Exists(filePath))
            {
                return Result<List<Matchup>, SystemError<XmlDataServiceProvider>>.Err(new SystemError<XmlDataServiceProvider>()
                    {
                        ErrorMessage = "Game data does not exist for the specified season.",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });
            }
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);
            fs.Close();

            var matchups = doc.Descendants("Game").Select(g => new Matchup
            {
                Id = (int)g.Element("Id")!,
                Season = int.Parse(g.Attribute("Season")?.Value ?? "0"),
                Week = int.Parse(g.Attribute("Week")?.Value ?? "0"),
                GameTime = g.Element("GameTime")?.Value ?? DateTime.UtcNow.ToShortTimeString(),
                GameDate = g.Element("GameDate")?.Value ?? DateTime.UtcNow.ToShortDateString(),
                Opponents = new Opponent
                {
                    AwayTeam = new Team
                    {
                        Name = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Name")?.Value ?? "",
                        Score = int.Parse(g.Element("Opponents")?.Element("Away_Team")?.Attribute("Score")?.Value ?? "0"),
                        Abbreviation = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Abbr")?.Value ?? "",
                        Division = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Division")?.Value ?? "",
                        Record = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Record")?.Value ?? "",
                        LogoPath = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Logo_Path")?.Value ?? "",
                        Emoji = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Emoji")?.Value ?? ""
                    },
                    HomeTeam = new Team
                    {
                        Name = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Name")?.Value ?? "",
                        Score = int.Parse(g.Element("Opponents")?.Element("Home_Team")?.Attribute("Score")?.Value ?? "0"),
                        Abbreviation = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Abbr")?.Value ?? "",
                        Division = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Division")?.Value ?? "",
                        Record = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Record")?.Value ?? "",
                        LogoPath = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Logo_Path")?.Value ?? "",
                        Emoji = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Emoji")?.Value ?? ""

                    }
                }
            }).ToList();

            return Result<List<Matchup>, SystemError<XmlDataServiceProvider>>.Ok(matchups);
        }

        #endregion

        #region GET WEEK MATCHUP DATA

        public async Task<Result<List<Matchup>, SystemError<XmlDataServiceProvider>>> GetWeekMatchupDataAsync(string season, string week)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"game_data_{season}.xml");
            if (!File.Exists(filePath))
            {
                return Result<List<Matchup>, SystemError<XmlDataServiceProvider>>.Err(new SystemError<XmlDataServiceProvider>()
                {
                    ErrorMessage = "Game data does not exist for the specified season.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);
            fs.Close();
            var matchups = doc.Descendants("Game")
                .Where(x => x.Attribute("Week")!.Value.Equals(week)).Select(g => new Matchup
            {
                Id = (int)g.Element("Id")!,
                Season = int.Parse(g.Attribute("Season")?.Value ?? "0"),
                Week = int.Parse(g.Attribute("Week")?.Value ?? "0"),
                GameTime = g.Element("GameTime")?.Value ?? DateTime.UtcNow.ToShortTimeString(),
                GameDate = g.Element("GameDate")?.Value ?? DateTime.UtcNow.ToShortDateString(),
                Opponents = new Opponent
                {
                    AwayTeam = new Team
                    {
                        Name = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Name")?.Value ?? "",
                        Score = int.Parse(g.Element("Opponents")?.Element("Away_Team")?.Attribute("Score")?.Value ?? "0"),
                        Abbreviation = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Abbr")?.Value ?? "",
                        Division = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Division")?.Value ?? "",
                        Record = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Record")?.Value ?? "",
                        LogoPath = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Logo_Path")?.Value ?? "",
                        Emoji = g.Element("Opponents")?.Element("Away_Team")?.Attribute("Emoji")?.Value ?? ""
                    },
                    HomeTeam = new Team
                    {
                        Name = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Name")?.Value ?? "",
                        Score = int.Parse(g.Element("Opponents")?.Element("Home_Team")?.Attribute("Score")?.Value ?? "0"),
                        Abbreviation = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Abbr")?.Value ?? "",
                        Division = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Division")?.Value ?? "",
                        Record = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Record")?.Value ?? "",
                        LogoPath = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Logo_Path")?.Value ?? "",
                        Emoji = g.Element("Opponents")?.Element("Home_Team")?.Attribute("Emoji")?.Value ?? ""

                    }
                }
            }).ToList();

            return Result<List<Matchup>, SystemError<XmlDataServiceProvider>>.Ok(matchups);
        }

        #endregion
    }
}
