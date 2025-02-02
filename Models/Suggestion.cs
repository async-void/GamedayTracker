using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using GamedayTracker.Enums;

namespace GamedayTracker.Models
{
    public class Suggestion
    {
        public int Id { get; set; }
        public required GuildMember Author { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public SuggestionStatus Status { get; set; }
    }
}
