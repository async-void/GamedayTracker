using GamedayTracker.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Cache
{
    public static class PaginationCache
    {
        private static readonly Dictionary<ulong, PaginationEnvelope> _cache = [];
        private static readonly object _lock = new();
        public static void Store<T>(ulong messageId, T data)
        {
            lock (_lock)
            {
                _cache[messageId] = new PaginationEnvelope
                {
                    Data = data!,
                    Type = typeof(T).Name
                };

            }
        }
        public static PaginationEnvelope? Get(ulong messageId)
        {
            lock (_lock)
            {
                return _cache.TryGetValue(messageId, out var env) ? env : default;
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
