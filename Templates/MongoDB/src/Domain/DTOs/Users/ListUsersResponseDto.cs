namespace Domain.DTOs.Users;

public class ListUsersResponseDto : BaseListResponseDto<ListUserResponseDto>
{
    public ListUsersResponseDto(IEnumerable<ListUserResponseDto> data, int currentPage, int size, int totalItems)
        : base(data, currentPage, size, totalItems)
    {
    }

    public ListUsersResponseDto(ListUserResponseDto data) : base(data)
    {
    }

    public ListUsersResponseDto()
    {
    }
}