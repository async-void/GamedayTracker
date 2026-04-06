using DSharpPlus;
using GamedayTracker.Enums;
using GamedayTracker.Helpers;
using GamedayTracker.Models;
using GamedayTracker.Models.Tickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamedayTracker.Repositories
{
    public sealed class TicketStore
    {
        private readonly object _lock = new();
        private readonly string _filePath;
        private readonly Dictionary<ulong, Ticket> _byId = [];
        private readonly Dictionary<(ulong GuildId, ulong UserId), Ticket> _openTickets = [];

        public TicketStore(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        // ------------------------------------------------------------
        // Lookup Methods
        // ------------------------------------------------------------

        public Ticket? GetById(ulong ticketId)
        {
            lock (_lock)
            {
                _byId.TryGetValue(ticketId, out var t);
                return t;
            }
        }

        public Ticket? GetOpenForUser(ulong guildId, ulong userId)
        {
            lock (_lock)
            {
                _openTickets.TryGetValue((guildId, userId), out var t);
                return t;
            }
        }

        public Ticket? GetByThreadId(ulong threadId)
        {
            lock (_lock)
            {
                return _byId.Values.FirstOrDefault(t => t.ThreadId == threadId);
            }
        }

        // ------------------------------------------------------------
        // Creation
        // ------------------------------------------------------------

        public Ticket Create(
            ulong guildId,
            ulong userId,
            ulong threadId,
            TicketType type,
            ulong ticketId)
        {
            lock (_lock)
            {
                var ticket = new Ticket
                {
                    TicketId = ticketId,
                    GuildId = guildId,
                    UserId = userId,
                    ThreadId = threadId,
                    Type = type,
                    Status = TicketStatus.Open,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _byId[ticketId] = ticket;
                _openTickets[(guildId, userId)] = ticket;

                Save();
                return ticket;
            }
        }

        // ------------------------------------------------------------
        // Closing
        // ------------------------------------------------------------

        public void Close(ulong ticketId)
        {
            lock (_lock)
            {
                if (!_byId.TryGetValue(ticketId, out var ticket))
                    return;

                ticket.Status = TicketStatus.Closed;
                ticket.ClosedAt = DateTimeOffset.UtcNow;

                _openTickets.Remove((ticket.GuildId, ticket.UserId));

                Save();
            }
        }

        // ------------------------------------------------------------
        // Persistence
        // ------------------------------------------------------------

        private void Load()
        {
            if (!File.Exists(_filePath))
                return;

            var json = File.ReadAllText(_filePath);
            var list = JsonSerializer.Deserialize<List<Ticket>>(json) ?? [];

            foreach (var t in list)
            {
                _byId[t.TicketId] = t;

                if (t.Status == TicketStatus.Open)
                    _openTickets[(t.GuildId, t.UserId)] = t;
            }
        }

        private void Save()
        {
            var list = _byId.Values.OrderBy(t => t.CreatedAt).ToList();
            var json = JsonSerializer.Serialize(list, JsonHelper.DefaultJsonOptions);

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, json);
        }
    }
}
