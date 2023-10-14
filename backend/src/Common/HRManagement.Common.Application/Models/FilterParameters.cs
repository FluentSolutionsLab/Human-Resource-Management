namespace HRManagement.Common.Application.Models;

public class FilterParameters
{
    private const int MaxPageSize = 20;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = ComputePageNumber(value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = ComputePageSize(value);
    }

    public string SearchQuery { get; set; }

    private int ComputePageNumber(int value)
    {
        return value < 0 ? _pageNumber : value;
    }

    private int ComputePageSize(int value)
    {
        if (value < 0)
            return _pageSize;
        return value > MaxPageSize ? MaxPageSize : value;
    }
}