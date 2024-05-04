using Domain.DTOs.Users;

namespace Application.Services.Users;

public interface IUserDeactivator
{
    Task DeactivateAsync(DeactivateUserRequestDto request);
}