using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GamedayTracker.Data;
using GamedayTracker.Enums;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;

namespace GamedayTracker.Services
{
    public class ConfigurationDataService : IConfigurationData
    {
        public Result<string, SystemError<ConfigurationDataService>> GetBotPrefix()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "config.json");
            var content = File.ReadAllText(configPath);
            var json = JsonSerializer.Deserialize<ConfigJson>(content);
            var prefix = json!.Prefix!;

            if (prefix != "")
                return Result<string, SystemError<ConfigurationDataService>>.Ok(prefix);

            return Result<string, SystemError<ConfigurationDataService>>.Err(new SystemError<ConfigurationDataService>
            {
                ErrorMessage = "Could not find config.json file",
                ErrorType = ErrorType.WARNING,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }

        public Result<string, SystemError<ConfigurationDataService>> GetBotToken()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "config.json");
            var content = File.ReadAllText(configPath);
            var json = JsonSerializer.Deserialize<ConfigJson>(content);
            var token = json!.Token!;

            if (token != "")
                return Result<string, SystemError<ConfigurationDataService>>.Ok(token);

            return Result<string, SystemError<ConfigurationDataService>>.Err(new SystemError<ConfigurationDataService>
            {
                ErrorMessage = "Could not find config.json file",
                ErrorType = ErrorType.WARNING,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = this
            });
        }

        public Result<string, SystemError<AppDbContext>> GetConnectionString(string type)
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "config.json");
            var content = File.ReadAllText(configPath);
            var json = JsonSerializer.Deserialize<ConfigJson>(content);
            var conStr = json!.ConnectionStrings!.Default!;

            if (conStr != "")
                return Result<string, SystemError<AppDbContext>>.Ok(conStr);

            return Result<string, SystemError<AppDbContext>>.Err(new SystemError<AppDbContext>
            {
                ErrorMessage = "Could not find config.json file",
                ErrorType = ErrorType.WARNING,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = new AppDbContext(null)
            });
        }
    }
}
