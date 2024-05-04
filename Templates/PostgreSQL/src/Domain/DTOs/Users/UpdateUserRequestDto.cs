namespace Domain.DTOs.Users;

public class UpdateUserRequestDto(
    string email,
    string oldPassword,
    string newPassword,
    string requestedBy,
    CancellationToken token)
    : BaseRequestDto(requestedBy, token)
{
    public string Email { get; } = email;
    public string OldPassword { get; } = oldPassword;
    public string NewPassword { get; } = newPassword;
}