using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamedayTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GamedayTracker.Configuration.EntityTypeConfiguration
{
    public class BetEntityTypeConfiguration : IEntityTypeConfiguration<Bet>
    {
        public void Configure(EntityTypeBuilder<Bet> builder)
        {
            builder.Property(x => x.Matchup.Opponents.AwayTeam.Name).HasMaxLength(50);
            builder.Property(x => x.Matchup.Opponents.HomeTeam.Name).HasMaxLength(50);
        }
    }
}
