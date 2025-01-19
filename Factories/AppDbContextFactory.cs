using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Data;
using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using GamedayTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace GamedayTracker.Factories
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private readonly ConfigurationDataService _dataService = new();
        public AppDbContext CreateDbContext(string[]? args = null)
        {
            var result = _dataService.GetConnectionString("default");
            if (result.IsOk)
            {
                var conStr = result.Value;
                var options = new DbContextOptionsBuilder();
                options.UseNpgsql(conStr);
                return new AppDbContext(options.Options);
            }
            else
            {
                return null;
            }
        }
    }
}
