namespace HRManagement.Api.Models;

public class PaginationParameters
{
    private const int MaxPageSize = 20;
    private int _pageSize = 10;
    private int _pageNumber = 1;

    public int? PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = ComputePageNumber(value);
    }

    public int? PageSize
    {
        get => _pageSize;
        set => _pageSize = ComputePageSize(value);
    }

    private int ComputePageNumber(int? value)
    {
        return value is null or < 0 ? _pageNumber : value.Value;
    }

    private int ComputePageSize(int? value)
    {
        if (value is null or < 0)
            return _pageSize;
        return value.Value > MaxPageSize ? MaxPageSize : value.Value;
    }
}