using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Data;
using GamedayTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GamedayTracker.Factories
{
    public class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext>
    {
        private readonly ConfigurationDataService _dataService = new();

        public BotDbContext CreateDbContext(string[]? args = null)
        {
            var result = _dataService.GetConnectionString("gameday");
            if (result.IsOk)
            {
                var conStr = result.Value;
                var options = new DbContextOptionsBuilder();
                options.UseNpgsql(conStr);
                return new BotDbContext(options.Options);
            }
            else
            {
                return null;
            }
        }
    }
}
