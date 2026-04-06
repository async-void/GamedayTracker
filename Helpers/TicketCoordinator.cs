using GamedayTracker.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public sealed class TicketCoordinator
    {
        private readonly Dictionary<ulong, PendingJoin> _pendingJoins = [];

        public void AddPendingJoin(PendingJoin join)
            => _pendingJoins[join.UserId] = join;

        public bool TryGetPendingJoin(ulong userId, out PendingJoin? join)
            => _pendingJoins.TryGetValue(userId, out join);

        public void RemovePendingJoin(ulong userId)
            => _pendingJoins.Remove(userId);

    }
}
