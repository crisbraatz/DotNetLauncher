namespace Presentation.Contracts;

/// <summary>
/// Base list response.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public abstract record BaseListResponse<T>
{
    /// <summary>
    /// Data.
    /// </summary>
    public IEnumerable<T> Data { get; init; }

    /// <summary>
    /// Current page.
    /// </summary>
    public int CurrentPage { get; init; }

    /// <summary>
    /// Total pages.
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Total items.
    /// </summary>
    public int TotalItems { get; init; }
}