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
    public class PoolPlayerEntityTypeConfiguration : IEntityTypeConfiguration<PoolPlayer>
    {
        public void Configure(EntityTypeBuilder<PoolPlayer> builder)
        {
            builder.Property(x => x.PlayerName).HasMaxLength(100);
            builder.Property(x => x.Company).HasMaxLength(100);
        }
    }
}
