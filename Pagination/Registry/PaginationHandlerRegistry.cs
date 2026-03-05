using GamedayTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Pagination.Registry
{
    public static class PaginationHandlerRegistry
    {
        private static readonly Dictionary<Type, IPaginationHandler> _handlers = new();

        public static void Register<T>(IPaginationHandler handler)
        {
            _handlers[typeof(T)] = handler;
        }

        public static IPaginationHandler? GetHandler(Type t)
        {
            return _handlers.TryGetValue(t, out var handler) ? handler : null;
        }
    }
}
