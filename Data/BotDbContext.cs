using GamedayTracker.Configuration.EntityTypeConfiguration;
using GamedayTracker.Models;
using GamedayTracker.Models.DailyNumbers;
using Microsoft.EntityFrameworkCore;

namespace GamedayTracker.Data
{
    public class BotDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<GuildMember> Members { get; set; }
        public DbSet<Guild>? Guilds { get; set; }
        public DbSet<PoolPlayer> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuildMemberEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PoolPlayerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BetEntityTypeConfiguration());

            modelBuilder.Entity<GuildMember>()
                        .HasOne(g => g.Bank)
                        .WithOne(b => b.GuildMember)
                        .HasForeignKey<Bank>(b => b.GuildMemberId);

            modelBuilder.Entity<GuildMember>()
                        .OwnsMany(g => g.DailyNumbers);


            base.OnModelCreating(modelBuilder);
        }
    }
}
