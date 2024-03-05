namespace HRManagement.BuildingBlocks.Models;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int) Math.Ceiling(count / (double) pageSize);
        AddRange(items);
    }

    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = await Task.FromResult(source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList());
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}