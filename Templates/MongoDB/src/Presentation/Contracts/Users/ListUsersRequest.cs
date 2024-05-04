using Domain.DTOs.Users;
using Presentation.Helpers;

namespace Presentation.Contracts.Users;

/// <summary>
/// List users request.
/// </summary>
public record ListUsersRequest : BaseListRequest
{
    /// <summary>
    /// Filter by email.
    /// </summary>
    public string Email { get; init; }

    public ListUsersRequestDto ConvertToDto(string requestedBy, CancellationToken token) => new(requestedBy, token)
    {
        Email = Email,
        Id = Id,
        CreatedAt = CreatedAt,
        CreatedBy = CreatedBy,
        UpdatedAt = UpdatedAt,
        UpdatedBy = UpdatedBy,
        Active = Active,
        OrderBy = OrderByHelper.ToDictionary<ListUserResponseDto>(OrderBy),
        Page = Page,
        Size = Size
    };
}