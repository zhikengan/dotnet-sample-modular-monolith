namespace App.Shared.Models;

public class PagedResponse<T> : StdResponse<IEnumerable<T>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PagedResponse<T> Ok(IEnumerable<T> data, int pageIndex, int pageSize, long totalCount)
    {
        return new PagedResponse<T>
        {
            Success = true,
            Data = data,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
