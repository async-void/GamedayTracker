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
    public class GuildMemberEntityTypeConfiguration : IEntityTypeConfiguration<GuildMember>
    {
        public void Configure(EntityTypeBuilder<GuildMember> builder)
        {
            builder.Property(x => x.MemberName).HasMaxLength(100);
            builder.Property(x => x.MemberId).HasMaxLength(100);
            builder.Property(x => x.FavoriteTeam).HasMaxLength(100);
        }
    }
}
