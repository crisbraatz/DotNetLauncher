namespace Presentation.Contracts;

/// <summary>
/// Base list request.
/// </summary>
public abstract record BaseListRequest : BaseList
{
    /// <summary>
    /// Page order by.
    /// Default: id asc.
    /// </summary>
    public string OrderBy { get; init; } = "id asc";

    /// <summary>
    /// Page number.
    /// Default: 1.
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Total page items.
    /// Default: 10.
    /// </summary>
    public int Size { get; init; } = 10;
}