using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Models.Pagination
{
    public class PaginatedList<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int LastPageIndex => TotalPages - 1;

        public PaginatedList(IEnumerable<T> items, int pageSize) 
        {
            Items = items.ToList();
            PageSize = pageSize;
            TotalItems = Items.Count;

            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
        }

        public PaginationResult<T> GetPage(int pageIndex)
        {
            if (TotalPages == 0)
                return new PaginationResult<T>([], 0, -1);

            pageIndex = Math.Clamp(pageIndex, 0, LastPageIndex);
            var slice = Items.Skip(pageIndex * PageSize)
                             .Take(PageSize)
                             .ToList();

            return new PaginationResult<T>(slice, pageIndex, LastPageIndex);
        }
    }
}
