using Application.Exceptions;
using Application.Services.Users;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using FluentAssertions;

namespace Integration.Application.Services.Users;

public class UserAuthenticatorTests : IntegrationBase
{
    private readonly IBaseEntityRepository<User> _repository;
    private readonly IUserAuthenticator _authenticator;
    private readonly User _user = new("example@template.com", "example@template.com".GetHashedPassword("Example123"));
    private AuthenticateUserRequestDto _request;
    private Func<Task> _func;

    private AuthenticateUserRequestDto BuildRequest(
        string email = "example@template.com", string password = "Example123", string requestedBy = null) =>
        new(email, password, requestedBy, Token);

    public UserAuthenticatorTests()
    {
        _repository = GetRequiredService<IBaseEntityRepository<User>>();
        _authenticator = GetRequiredService<IUserAuthenticator>();
    }

    [Fact]
    public async Task Should_authenticate_user()
    {
        await _repository.InsertOneAsync(_user);
        await CommitAsync();

        _func = () => _authenticator.AuthenticateAsync(BuildRequest());

        await _func.Should().NotThrowAsync<Exception>();
    }

    [Fact]
    public async Task Should_reauthenticate_user()
    {
        await _repository.InsertOneAsync(_user);
        await CommitAsync();

        _func = () => _authenticator.AuthenticateAsync(BuildRequest(requestedBy: RequestedBy));

        await _func.Should().NotThrowAsync<Exception>();
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_email_format()
    {
        _func = () => _authenticator.AuthenticateAsync(BuildRequest(email: "example@template.com."));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_throw_exception_when_authenticated_user_different_from_request_user()
    {
        _request = BuildRequest(email: "another.example@template.com", requestedBy: RequestedBy);

        _func = () => _authenticator.AuthenticateAsync(_request);

        await _func.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage(
            $"Authenticated user {_request.RequestedBy} different from request user {_request.Email}.");
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_password_format()
    {
        _func = () => _authenticator.AuthenticateAsync(BuildRequest(password: "Example"));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid password format.");
    }

    [Fact]
    public async Task Should_throw_exception_when_user_not_found()
    {
        _request = BuildRequest();

        _func = () => _authenticator.AuthenticateAsync(_request);

        await _func.Should().ThrowAsync<EntityNotFoundException>().WithMessage($"User {_request.Email} not found.");
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_password()
    {
        await _repository.InsertOneAsync(_user);
        await CommitAsync();
        _request = BuildRequest(password: "Example1234");

        _func = () => _authenticator.AuthenticateAsync(_request);

        await _func.Should().ThrowAsync<InvalidPropertyValueException>()
            .WithMessage($"Invalid password for user {_request.Email}.");
    }
}