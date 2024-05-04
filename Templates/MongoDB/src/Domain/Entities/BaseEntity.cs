namespace Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string UpdatedBy { get; private set; }
    public bool Active { get; private set; }

    protected void SetActive(string requestedBy)
    {
        SetUpdate(requestedBy);
        Active = true;
    }

    protected void SetCreate(string requestedBy)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        CreatedBy = requestedBy;
        SetActive(requestedBy);
    }

    protected void SetInactive(string requestedBy)
    {
        SetUpdate(requestedBy);
        Active = false;
    }

    protected void SetUpdate(string requestedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = requestedBy;
    }
}