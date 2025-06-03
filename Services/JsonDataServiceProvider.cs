using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.Services
{
    public class JsonDataServiceProvider : IJsonDataService
    {
        #region GET MATCHUPS
        public async Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> GetMatchupsAsync(string season, string week)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "matchups.json");

            if (!File.Exists(path))
            {
                var notFound = new SystemError<JsonDataServiceProvider>
                {
                    ErrorMessage = "Matchup data file not found.",
                    CreatedAt = DateTime.UtcNow,
                    ErrorType = ErrorType.INFORMATION,
                    CreatedBy = this
                };
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(notFound);
            }
            var json = await File.ReadAllTextAsync(path);
            var matchups = JsonSerializer.Deserialize<List<Matchup>>(json);

            if (matchups is not null) return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(matchups);
            var error = new SystemError<JsonDataServiceProvider>
            {
                ErrorMessage = "Failed to fetch matchup data.",
                CreatedAt = DateTime.UtcNow,
                ErrorType = ErrorType.INFORMATION,
                CreatedBy = this
            };
            return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Err(error);
        }
        #endregion

        #region WRITE MATCHUP TO JSON
        public async Task<Result<bool, SystemError<JsonDataServiceProvider>>> WriteMatchupToJson(Matchup matchup)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "matchups.json");

            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(matchup, options);
                await File.WriteAllTextAsync(path, json);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var json = JsonSerializer.Deserialize<List<Matchup>>(file);
                json!.Add(matchup);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(json, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<bool, SystemError<JsonDataServiceProvider>>.Ok(true);
            }
        }
        #endregion

        #region WRITE ALL MATCHUPS TO JSON
        public async Task<Result<List<Matchup>, SystemError<JsonDataServiceProvider>>> WriteAllMatchupsToJson(List<Matchup> matchups)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Json", "matchups.json");
            if (!File.Exists(path))
            {
                var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve};
                var json = JsonSerializer.Serialize(matchups, options);
                await File.WriteAllTextAsync(path, json);
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(matchups);
            }
            else
            {
                var file = await File.ReadAllTextAsync(path);
                var existingMatchups = JsonSerializer.Deserialize<List<Matchup>>(file);
                existingMatchups!.AddRange(matchups);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var updatedJson = JsonSerializer.Serialize(existingMatchups, options);
                await File.WriteAllTextAsync(path, updatedJson);
                return Result<List<Matchup>, SystemError<JsonDataServiceProvider>>.Ok(existingMatchups);
            }
        }
        #endregion
    }
}
