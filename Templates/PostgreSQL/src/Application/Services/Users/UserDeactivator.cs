using Application.Exceptions;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using Infrastructure.RabbitMq;
using Microsoft.Extensions.Logging;

namespace Application.Services.Users;

public class UserDeactivator(
    IBaseEntityRepository<User> repository,
    ILogger<UserDeactivator> logger,
    BasePublisher publisher)
    : IUserDeactivator
{
    public async Task DeactivateAsync(DeactivateUserRequestDto request)
    {
        var email = request.Email.ToLowerInvariant();

        EmailHelper.ValidateFormat(email);

        if (!request.RequestedBy.Equals(email, StringComparison.InvariantCultureIgnoreCase))
        {
            logger.LogError("Authenticated user {requestedBy} different from request user {email}.",
                request.RequestedBy, email);
            throw new UnauthorizedAccessException(
                $"Authenticated user {request.RequestedBy} different from request user {email}.");
        }

        PasswordHelper.ValidateFormat(request.Password);

        var userDb = await repository.SelectOneByAsync(x => x.Email == email && x.Active, true, token: request.Token);

        if (userDb is null)
        {
            logger.LogError("User {email} not found.", email);
            throw new EntityNotFoundException($"User {email} not found.");
        }

        if (!userDb.Email.IsMatch(userDb.Password, request.Password))
        {
            logger.LogError("Invalid password for user {email}.", email);
            throw new InvalidPropertyValueException($"Invalid password for user {email}.");
        }

        userDb.Deactivate(email);

        repository.DeleteOne(userDb);

        publisher.Publish(nameof(ExchangeEnum.UserEntityRequestsExchange), 1);
    }
}