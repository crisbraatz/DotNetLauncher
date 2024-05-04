using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.DTOs;

public abstract class BaseListRequestDto<T>(string requestedBy = null, CancellationToken token = default)
    : BaseListDto where T : BaseEntity
{
    public HashSet<Expression<Func<T, bool>>> Filters { get; init; } = [];
    public IDictionary<string, bool> OrderBy { get; init; } = new Dictionary<string, bool>();
    public string RequestedBy { get; } = requestedBy;
    public CancellationToken Token { get; } = token;

    public int Page
    {
        get => _page;
        init => _page = value > 1 ? value : 1;
    }

    public int Size
    {
        get => _size;
        init => _size = value >= 1 ? int.Min(value, 100) : 10;
    }

    private readonly int _page = 1;
    private readonly int _size = 10;
}