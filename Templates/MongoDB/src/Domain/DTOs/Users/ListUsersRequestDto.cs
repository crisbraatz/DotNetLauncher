using Domain.Entities.Users;

namespace Domain.DTOs.Users;

public class ListUsersRequestDto(string requestedBy, CancellationToken token)
    : BaseListRequestDto<User>(requestedBy, token)
{
    public string Email { get; init; }
}