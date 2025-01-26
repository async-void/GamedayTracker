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
    public class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {

            builder.Property(x => x.Name)
                .HasMaxLength(25);
            builder.Property(x => x.Abbreviation)
                .HasMaxLength(10);
            builder.Property(x => x.Division)
                .HasMaxLength(25);
            builder.Property(x => x.Record)
                .HasMaxLength(15);
            builder.Property(x => x.LogoPath)
                .HasMaxLength(500);
            builder.Property(x => x.Emoji)
                .HasMaxLength(100);
        }
    }
}
