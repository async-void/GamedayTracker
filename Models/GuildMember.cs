﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace GamedayTracker.Models
{
    public class GuildMember
    {
        public int Id { get; set; }
        public required string MemberName { get; set; }
        public required string MemberId { get; set; }
        public required string GuildId { get; set; } 
        public required string GuildName { get; set; }
        public string? FavoriteTeam { get; set; }
        public double? Balance { get; set; }
        public DateTime? LastDeposit { get; set; }

    }
}
