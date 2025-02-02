using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Data;
using Microsoft.EntityFrameworkCore.Design;

namespace GamedayTracker.Factories
{
    public class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext>
    {
        public BotDbContext CreateDbContext(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
