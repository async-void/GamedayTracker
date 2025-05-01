using GamedayTracker.Configuration.EntityTypeConfiguration;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GamedayTracker.Data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        //DbSet<T> here
        public DbSet<Matchup> Matchups { get; set; }
        public DbSet<TeamStanding> TeamStandings { get; set; }
        public DbSet<DraftEntity> DraftEntities { get; set; }
        public DbSet<TeamStats> TeamStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TeamEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TeamStandingEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new GuildMemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DraftEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TeamStatsEntityTypeConfiguration());
        }
    }
}
