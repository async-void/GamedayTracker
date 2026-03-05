using GamedayTracker.Pagination;

namespace GamedayTracker.Cache
{
    public static class TeamStatsPaginationCache
    {
        private static readonly Dictionary<ulong, TeamStatsPaginationData> _cache = [];
        private static readonly object _lock = new();

        public static void Store(ulong messageId, TeamStatsPaginationData data)
        {
            lock (_lock)
            {
                _cache[messageId] = data;
            }
        }

        public static TeamStatsPaginationData? Get(ulong messageId)
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
