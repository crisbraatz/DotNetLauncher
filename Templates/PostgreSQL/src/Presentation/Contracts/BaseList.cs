namespace Presentation.Contracts;

/// <summary>
/// Base list.
/// </summary>
public abstract record BaseList
{
    /// <summary>
    /// ID.
    /// </summary>
    public Guid? Id { get; init; }

    /// <summary>
    /// Date of creation. 
    /// </summary>
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Author of creation.
    /// </summary>
    public string CreatedBy { get; init; }

    /// <summary>
    /// Date of update.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Author of update.
    /// </summary>
    public string UpdatedBy { get; init; }

    /// <summary>
    /// Is active?
    /// </summary>
    public bool? Active { get; init; }
}