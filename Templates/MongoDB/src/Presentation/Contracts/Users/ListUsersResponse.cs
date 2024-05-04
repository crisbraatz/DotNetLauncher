using Domain.DTOs.Users;

namespace Presentation.Contracts.Users;

/// <summary>
/// List users response.
/// </summary>
public record ListUsersResponse : BaseListResponse<ListUserResponse>
{
    public static ListUsersResponse ConvertFromDto(ListUsersResponseDto dto) => new()
    {
        Data = dto.Data.Select(x => new ListUserResponse
        {
            Email = x.Email,
            Id = x.Id,
            CreatedAt = x.CreatedAt,
            CreatedBy = x.CreatedBy,
            UpdatedAt = x.UpdatedAt,
            UpdatedBy = x.UpdatedBy,
            Active = x.Active
        }),
        CurrentPage = dto.CurrentPage,
        TotalPages = dto.TotalPages,
        TotalItems = dto.TotalItems
    };
}