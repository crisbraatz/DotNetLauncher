namespace Domain.DTOs.Users;

public class DeactivateUserRequestDto(string email, string password, string requestedBy, CancellationToken token)
    : BaseRequestDto(requestedBy, token)
{
    public string Email { get; } = email;
    public string Password { get; } = password;
}