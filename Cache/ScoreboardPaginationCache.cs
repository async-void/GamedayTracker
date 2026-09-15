using GamedayTracker.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Cache
{
    public class ScoreboardPaginationCache
    {
        private static readonly Dictionary<ulong, PaginationData> _cache = [];
        private static readonly object _lock = new();

        public static void Store(ulong messageId, PaginationData data)
        {
            lock (_lock)
            {
                _cache[messageId] = data;
            }
        }

        public static PaginationData? Get(ulong messageId)
        {
            lock (_lock)
            {
                return _cache.TryGetValue(messageId, out var data) ? data : null;
            }
        }

        public static void Remove(ulong messageId)
        {
            lock (_lock)
            {
                _cache.Remove(messageId);
            }
        }
    }
}
