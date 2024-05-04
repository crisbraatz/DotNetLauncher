using Domain.DTOs.Users;

namespace Application.Services.Users;

public interface IUserAuthenticator
{
    Task AuthenticateAsync(AuthenticateUserRequestDto request);
}