using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GamedayTracker.Configuration.EntityTypeConfiguration
{
    public class TeamStandingEntityTypeConfiguration : IEntityTypeConfiguration<TeamStanding>
    {
        public void Configure(EntityTypeBuilder<TeamStanding> builder)
        {
            builder.Property(x => x.Division).HasMaxLength(20);
            builder.Property(x => x.Abbr).HasMaxLength(20);
            builder.Property(x => x.Loses).HasMaxLength(20);
            builder.Property(x => x.Pct).HasMaxLength(20);
            builder.Property(x => x.TeamName).HasMaxLength(50);
            builder.Property(x => x.Ties).HasMaxLength(20);
            builder.Property(x => x.Wins).HasMaxLength(20);
        }
    }
}
