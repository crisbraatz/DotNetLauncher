using Domain.DTOs.Users;

namespace Application.Services.Users;

public interface IUserPasswordUpdater
{
    Task UpdatePasswordAsync(UpdateUserRequestDto request);
}