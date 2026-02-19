namespace GamedayTracker.Models.Pagination
{
    public sealed class PaginationResult<T>
    {
        public IReadOnlyList<T> PageItems { get; }
        public int PageIndex { get; }
        public int LastPageIndex { get; }

        public PaginationResult(IReadOnlyList<T> pageItems, int pageIndex, int lastPageIndex)
        {
            PageItems = pageItems;
            PageIndex = pageIndex;
            LastPageIndex = lastPageIndex;
        }
    }
}
