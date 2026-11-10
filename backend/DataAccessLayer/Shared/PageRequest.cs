namespace DataAccessLayer.Shared
{
    public record PageRequest
    {
        public int PageSize { get; init; } = 20;
        public int? CurrentPage { get; init; }
    }
}
