using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Data
{
    public class BotDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<GuildMember> Members { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
