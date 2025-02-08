using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Data;
using GamedayTracker.Enums;
using GamedayTracker.Models;
using GamedayTracker.Services;

namespace GamedayTracker.Interfaces
{
    public interface IConfigurationData
    {
        Result<string, SystemError<ConfigurationDataService>> GetConnectionString(ConnectionStringType type);
        Result<string, SystemError<ConfigurationDataService>> GetBotToken();
        Result<string, SystemError<ConfigurationDataService>> GetBotPrefix();

    }
}
