using Domain.DTOs.Users;

namespace Application.Services.Users;

public interface IUserCreator
{
    Task CreateAsync(CreateUserRequestDto request);
}