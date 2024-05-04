using Application.Exceptions;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using Infrastructure.RabbitMq;
using Microsoft.Extensions.Logging;

namespace Application.Services.Users;

public class UserCreator(IBaseEntityRepository<User> repository, ILogger<UserCreator> logger, BasePublisher publisher)
    : IUserCreator
{
    public async Task CreateAsync(CreateUserRequestDto request)
    {
        if (!string.IsNullOrWhiteSpace(request.RequestedBy))
        {
            logger.LogError("Authenticated user {email} can not request user creation.", request.RequestedBy);
            throw new UnauthorizedAccessException(
                $"Authenticated user {request.RequestedBy} can not request user creation.");
        }

        var email = request.Email.ToLowerInvariant();

        EmailHelper.ValidateFormat(email);
        PasswordHelper.ValidateFormat(request.Password);

        if (await repository.ExistsByAsync(x => x.Email == email && x.Active, request.Token))
        {
            logger.LogError("User {email} already created.", email);
            throw new EntityAlreadyExistsException($"User {email} already created.");
        }

        await repository.InsertOneAsync(new User(email, email.GetHashedPassword(request.Password)), request.Token);

        publisher.Publish(nameof(ExchangeEnum.UserEntityRequestsExchange), 1);
    }
}