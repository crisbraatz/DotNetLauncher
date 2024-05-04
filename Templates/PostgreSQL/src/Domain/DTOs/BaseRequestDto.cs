namespace Domain.DTOs;

public abstract class BaseRequestDto(string requestedBy = null, CancellationToken token = default)
{
    public string RequestedBy { get; } = requestedBy;
    public CancellationToken Token { get; } = token;
}