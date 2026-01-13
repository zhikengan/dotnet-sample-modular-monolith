namespace App.Shared.Models;

public class StdPagedResponse<T> : StdResponse<IEnumerable<T>>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public static StdPagedResponse<T> Ok(IEnumerable<T> data, int page, int pageSize, int totalCount)
    {
        return new StdPagedResponse<T>
        {
            Success = true,
            Data = data,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
