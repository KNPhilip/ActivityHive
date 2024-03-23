using Microsoft.EntityFrameworkCore;

namespace Application.Core;

public sealed class PagedList<T> : List<T>
{
    private int currentPage;
    private int totalPages;
    private int pageSize;
    private int totalCount;

    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        AddRange(items);
    }

    public int CurrentPage 
    {
        get => currentPage;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            currentPage = value;
        }
    }

    public int TotalPages 
    {
        get => totalPages;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            totalPages = value;
        }
    }

    public int PageSize 
    {
        get => pageSize;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            pageSize = value;
        }
    }

    public int TotalCount 
    {
        get => totalCount;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(value));
            totalCount = value;
        }
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        int count = await source.CountAsync();
        List<T>? items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
