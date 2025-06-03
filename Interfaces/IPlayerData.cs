using GamedayTracker.Models;
using GamedayTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Interfaces
{
    public interface IPlayerData
    {
        Task<Result<bool, SystemError<PlayerDataServiceProvider>>> WritePlayerToXmlAsync(PoolPlayer player);
        Task<Result<PoolPlayer, SystemError<PlayerDataServiceProvider>>> GetPlayerFromXmlAsync(string playerName);
        Task<Result<int, SystemError<PlayerDataServiceProvider>>> GeneratePlayerIdAsync();
        Result<ulong, SystemError<PlayerDataServiceProvider>> GeneratePlayerIdentifier();
    }
}
