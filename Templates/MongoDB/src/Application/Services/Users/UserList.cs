using Application.Observability;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;

namespace Application.Services.Users;

public class UserList(IBaseEntityRepository<User> repository) : IUserList
{
    public async Task<ListUsersResponseDto> ListByAsync(ListUsersRequestDto request)
    {
        Meter.UserEntityRequests.Add(1);

        if (request.Id.HasValue)
            return await ListUserByAsync(request.Id.Value, request.Token);

        return await ListUsersByAsync(request);
    }

    private async Task<ListUsersResponseDto> ListUserByAsync(Guid id, CancellationToken token)
    {
        var data = await repository.ProjectOneByAsync(x => new ListUserResponseDto
            {
                Email = x.Email,
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                Active = x.Active
            },
            x => x.Id == id,
            token);

        return data is not null ? new ListUsersResponseDto(data) : new ListUsersResponseDto();
    }

    private async Task<ListUsersResponseDto> ListUsersByAsync(ListUsersRequestDto request)
    {
        if (!string.IsNullOrWhiteSpace(request.Email))
            request.Filters.Add(x => x.Email.Contains(request.Email.ToLowerInvariant()));

        if (request.Id.HasValue)
            request.Filters.Add(x => x.Id == request.Id);

        if (request is { CreatedAt: not null, UpdatedAt: null })
            request.Filters.Add(x => x.CreatedAt.Date == request.CreatedAt.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.CreatedBy))
            request.Filters.Add(x => x.CreatedBy.Contains(request.CreatedBy.ToLowerInvariant()));

        if (request is { UpdatedAt: not null, CreatedAt: null })
            request.Filters.Add(x => x.UpdatedAt.Date == request.UpdatedAt.Value.Date);

        if (!string.IsNullOrWhiteSpace(request.UpdatedBy))
            request.Filters.Add(x => x.UpdatedBy.Contains(request.UpdatedBy.ToLowerInvariant()));

        if (request.Active.HasValue)
            request.Filters.Add(x => x.Active == request.Active);

        var (data, total) = await repository.ListByAsync(request, x => new ListUserResponseDto
            {
                Email = x.Email,
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy,
                Active = x.Active
            },
            request.Token);

        return new ListUsersResponseDto(data, request.Page, request.Size, total);
    }
}