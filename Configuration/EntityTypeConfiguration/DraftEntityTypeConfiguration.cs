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
    public class DraftEntityTypeConfiguration : IEntityTypeConfiguration<DraftEntity>
    {
        public void Configure(EntityTypeBuilder<DraftEntity> builder)
        {
            builder.Property(x => x.College).HasMaxLength(100);
            builder.Property(x => x.PickPosition).HasMaxLength(12);
            builder.Property(x => x.PlayerName).HasMaxLength(100);
            builder.Property(x => x.Pos).HasMaxLength(12);
            builder.Property(x => x.Round).HasMaxLength(12);
            builder.Property(x => x.TeamName).HasMaxLength(100);
        }
    }
}
