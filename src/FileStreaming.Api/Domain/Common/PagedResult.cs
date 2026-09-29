namespace FileStreaming.Api.Domain.Common;

public sealed class PagedResult<T>(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
{
    public IReadOnlyList<T> Items { get; } = items;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = totalCount;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult<TResult> Map<TResult>(Func<T, TResult> map) =>
        new([.. Items.Select(map)], Page, PageSize, TotalCount);
}
