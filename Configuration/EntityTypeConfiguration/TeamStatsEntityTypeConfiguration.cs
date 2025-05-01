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
    public class TeamStatsEntityTypeConfiguration : IEntityTypeConfiguration<TeamStats>
    {
        public void Configure(EntityTypeBuilder<TeamStats> builder)
        {
            builder.Property(x => x.TeamName).HasMaxLength(200);
        }
    }
}
