using GamedayTracker.Enums;

namespace GamedayTracker.Models.Tickets
{
    public class Ticket
    {
        public ulong TicketId { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public ulong ThreadId { get; set; }
        public string Description { get; set; } = string.Empty;
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }

        // Optional future fields:
        public string? Summary { get; set; }
        public string? Details { get; set; }
    }
}
