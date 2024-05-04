using Application.Exceptions;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using Infrastructure.RabbitMq;
using Microsoft.Extensions.Logging;

namespace Application.Services.Users;

public class UserPasswordUpdater(
    IBaseEntityRepository<User> repository,
    ILogger<UserPasswordUpdater> logger,
    BasePublisher publisher)
    : IUserPasswordUpdater
{
    public async Task UpdatePasswordAsync(UpdateUserRequestDto request)
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

        PasswordHelper.ValidateFormat(request.OldPassword);
        PasswordHelper.ValidateFormat(request.NewPassword);

        if (request.OldPassword == request.NewPassword)
        {
            logger.LogError("Old and new password are equal for user {email}.", email);
            throw new InvalidPropertyValueException($"Old and new password are equal for user {email}.");
        }

        var userDb = await repository.SelectOneByAsync(x => x.Email == email && x.Active, true, token: request.Token);

        if (userDb is null)
        {
            logger.LogError("User {email} not found.", email);
            throw new EntityNotFoundException($"User {email} not found.");
        }

        if (!userDb.Email.IsMatch(userDb.Password, request.OldPassword))
        {
            logger.LogError("Invalid old password for user {email}.", email);
            throw new InvalidPropertyValueException($"Invalid old password for user {email}.");
        }

        userDb.UpdatePassword(email.GetHashedPassword(request.NewPassword), email);

        repository.UpdateOne(userDb);

        publisher.Publish(nameof(ExchangeEnum.UserEntityRequestsExchange), 1);
    }
}