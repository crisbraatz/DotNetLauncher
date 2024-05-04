namespace Domain.DTOs;

public abstract class BaseListResponseDto<T>(IEnumerable<T> data, int currentPage, int size, int totalItems)
{
    public IEnumerable<T> Data { get; } = data;
    public int CurrentPage { get; } = currentPage;
    public int TotalPages { get; } = (int)Math.Ceiling((double)(totalItems > 0 ? totalItems : 1) / size);
    public int TotalItems { get; } = totalItems;

    protected BaseListResponseDto(T data) : this(new List<T> { data }, 1, 1, 1)
    {
    }

    protected BaseListResponseDto() : this(Enumerable.Empty<T>(), 1, 1, 0)
    {
    }
}