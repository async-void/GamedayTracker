using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using GamedayTracker.Configuration.EntityTypeConfiguration;

namespace GamedayTracker.Data
{
    public class BotDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<GuildMember> Members { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<PoolPlayer> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuildMemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PoolPlayerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BetEntityTypeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
