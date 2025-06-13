using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GamedayTracker.Services
{
    public class PlayerDataServiceProvider : IPlayerData
    {
        #region GET PLAYER DATA FROM XML

        public async Task<Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>> GetPlayerFromXmlAsync(
            string playerName)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"player_data.xml");
            if (!File.Exists(filePath))
            {
                return Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>.Err(
                    new SystemError<PlayerDataServiceProvider>()
                    {
                        ErrorMessage = "Player data does not exist.Please use slash command ``/addplayer`` to add a new player",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);
            fs.Close();

            var player = doc.Descendants("Player")
                .FirstOrDefault(x => x.Attribute("Name")!.Value.Equals(playerName));
                  
            if (player is null)
            {
                return Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>.Err(
                    new SystemError<PlayerDataServiceProvider>()
                    {
                        ErrorMessage = "Player data could not be parsed.",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });
            }
            var newPlayer = new PoolPlayer
            {
                Id = int.Parse(player.Attribute("Id")!.Value),
                PlayerId = ulong.Parse(player.Attribute("Identifier")!.Value),
                PlayerName = player.Attribute("Name")!.Value,
                Company = player.Attribute("Company")!.Value,
                Balance = double.Parse(player.Element("Bank")!.Attribute("Balance")!.Value),
                DepositTimestamp = DateTime.Parse(player.Element("Bank")!.Attribute("Last_Deposit")!.Value)
            };
            return Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>.Ok(newPlayer);
        }
        #endregion

        #region WRITE PLAYER TO XML
        public async Task<Result<bool, SystemError<PlayerDataServiceProvider>>> WritePlayerToXmlAsync(PoolPlayer player)
        {
            var playerIdentifier = GeneratePlayerIdentifier();
            var id = await GeneratePlayerIdAsync();
            
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"player_data.xml");
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                
                //create new XmlDocument
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
                    new XElement("Players",
                        new XElement("Player",
                            new XAttribute("Id", id.Value),
                            new XAttribute("Identifier", playerIdentifier.Value),
                            new XAttribute("Name", player.PlayerName!),
                            new XAttribute("Company", player.Company!),
                            new XElement("Bank",
                                new XAttribute("Balance", player.Balance),
                                new XAttribute("Last_Deposit", DateTime.UtcNow)))));
                await using var stream = File.Create(filePath);
                await doc.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
                return Result<bool, SystemError<PlayerDataServiceProvider>>.Ok(true);
            }
            else
            {
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);
                fs.Close();

                var existingPlayer = doc.Descendants("Player")
                    .FirstOrDefault(p => p.Attribute("Name")!.Value.Equals(player.PlayerName));

                if (existingPlayer is not null)
                {
                    return Result<bool, SystemError<PlayerDataServiceProvider>>.Err(new SystemError<PlayerDataServiceProvider>()
                    {
                        ErrorMessage = "Player data already exists, did not over write player data.",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });
                }
                
                doc.Root?.Add(new XElement("Player",
                    new XAttribute("Id", id.Value),
                    new XAttribute("Identifier", playerIdentifier.Value),
                    new XAttribute("Name", player.PlayerName!),
                    new XAttribute("Company", player.Company!),
                    new XElement("Bank",
                        new XAttribute("Balance", player.Balance),
                        new XAttribute("last_deposit", player.DepositTimestamp))));

                await using var stream = File.Create(filePath);
                await doc.SaveAsync(stream, SaveOptions.None, CancellationToken.None);

                return Result<bool, SystemError<PlayerDataServiceProvider>>.Ok(true);
            }
        }
        #endregion

        #region GET PLAYER DATA FROM Json

        public async Task<Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>> GetPlayerFromJsonAsync(
            string playerName)
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", $"player_data.json");
            if (!File.Exists(filePath))
            {
                
                return Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>.Err(
                    new SystemError<PlayerDataServiceProvider>()
                    {
                        ErrorMessage = "Player data does not exist.Please use slash command ``/addplayer`` to add a new player",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);
            fs.Close();

            var player = doc.Descendants("Player")
                .FirstOrDefault(x => x.Attribute("Name")!.Value.Equals(playerName));

            if (player is null)
            {
                return Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>.Err(
                    new SystemError<PlayerDataServiceProvider>()
                    {
                        ErrorMessage = "Player data could not be parsed.",
                        ErrorType = ErrorType.INFORMATION,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = this
                    });
            }
            var newPlayer = new PoolPlayer
            {
                Id = int.Parse(player.Attribute("Id")!.Value),
                PlayerId = ulong.Parse(player.Attribute("Identifier")!.Value),
                PlayerName = player.Attribute("Name")!.Value,
                Company = player.Attribute("Company")!.Value,
                Balance = double.Parse(player.Element("Bank")!.Attribute("Balance")!.Value),
                DepositTimestamp = DateTime.Parse(player.Element("Bank")!.Attribute("Last_Deposit")!.Value)
            };
            return Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>.Ok(newPlayer);
        }
        #endregion

        #region GENERATE PLAYER ID
        public async Task<Result<int, SystemError<PlayerDataServiceProvider>>> GeneratePlayerIdAsync()
        {
            // ReSharper disable once StringLiteralTypo
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "XML", $"player_data.xml");
            if (!File.Exists(filePath))
            {
                return Result<int, SystemError<PlayerDataServiceProvider>>.Ok(1);
            }
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            var doc = await XDocument.LoadAsync(fs, LoadOptions.None, CancellationToken.None);
            fs.Close();
            var lastIdResult = int.TryParse(doc.Descendants("Player").Last().Attribute("Id")!.Value, out var idResult);
            
            if (lastIdResult)
                return Result<int, SystemError<PlayerDataServiceProvider>>.Ok(idResult + 1);
            return Result<int, SystemError<PlayerDataServiceProvider>>.Err(
                new SystemError<PlayerDataServiceProvider>()
                {
                    ErrorMessage = "Error Generating Player Id.",
                    ErrorType = ErrorType.INFORMATION,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = this
                });
        }
        #endregion

        #region GENERATE PLAYER IDENTIFIER
        public Result<ulong, SystemError<PlayerDataServiceProvider>> GeneratePlayerIdentifier()
        {
            var random = new Random();
            var buffer = new byte[8];
            random.NextBytes(buffer);
            var randomUlong = BitConverter.ToUInt64(buffer, 0);

            return Result<ulong, SystemError<PlayerDataServiceProvider>>.Ok(randomUlong);
        }
        #endregion

    }
}
