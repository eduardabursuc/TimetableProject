namespace Application.Utils;

public class PagedResult<T>(List<T> data, int totalCount)
{
    public List<T> Data { get; } = data;
    public int TotalCount { get; } = totalCount;
}