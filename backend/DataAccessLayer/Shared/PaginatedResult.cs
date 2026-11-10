namespace DataAccessLayer.Shared
{
    public class PaginatedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
    }
}
