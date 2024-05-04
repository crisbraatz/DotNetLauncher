using Domain.DTOs.Users;

namespace Application.Services.Users;

public interface IUserList
{
    Task<ListUsersResponseDto> ListByAsync(ListUsersRequestDto request);
}