using Application.Exceptions;
using Application.Services.Users;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using FluentAssertions;

namespace Integration.Application.Services.Users;

public class UserDeactivatorTests : IntegrationBase
{
    private readonly IBaseEntityRepository<User> _repository;
    private readonly IUserDeactivator _deactivator;

    private readonly User _userBefore =
        new("example@template.com", "example@template.com".GetHashedPassword("Example123"));

    private User _userAfter;
    private DeactivateUserRequestDto _request;
    private Func<Task> _func;

    private DeactivateUserRequestDto BuildRequest(
        string email = "example@template.com", string password = "Example123", string requestedBy = RequestedBy) =>
        new(email, password, requestedBy, Token);

    public UserDeactivatorTests()
    {
        _repository = GetRequiredService<IBaseEntityRepository<User>>();
        _deactivator = GetRequiredService<IUserDeactivator>();
    }

    [Fact]
    public async Task Should_deactivate_user()
    {
        await _repository.InsertOneAsync(_userBefore);
        _request = BuildRequest();

        await _deactivator.DeactivateAsync(_request);

        _userAfter = await _repository.SelectOneByAsync(x => x.Email == _request.Email);
        _userAfter.UpdatedAt.Should().BeAfter(_userAfter.CreatedAt);
        _userAfter.UpdatedBy.Should().Be(RequestedBy);
        _userAfter.Active.Should().BeFalse();
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_email_format()
    {
        _func = () => _deactivator.DeactivateAsync(
            BuildRequest(email: "example@template.com.", requestedBy: "example@template.com."));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_throw_exception_when_authenticated_user_different_from_request_user()
    {
        _request = BuildRequest(requestedBy: "another.example@template.com");

        _func = () => _deactivator.DeactivateAsync(_request);

        await _func.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage(
            $"Authenticated user {_request.RequestedBy} different from request user {_request.Email}.");
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_password_format()
    {
        _func = () => _deactivator.DeactivateAsync(BuildRequest(password: "Example"));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid password format.");
    }

    [Fact]
    public async Task Should_throw_exception_when_user_not_found()
    {
        _request = BuildRequest();

        _func = () => _deactivator.DeactivateAsync(_request);

        await _func.Should().ThrowAsync<EntityNotFoundException>().WithMessage($"User {_request.Email} not found.");
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_password()
    {
        await _repository.InsertOneAsync(_userBefore);
        _request = BuildRequest(password: "Example1234");

        _func = () => _deactivator.DeactivateAsync(_request);

        await _func.Should().ThrowAsync<InvalidPropertyValueException>()
            .WithMessage($"Invalid password for user {_request.Email}.");
        _userAfter = await _repository.SelectOneByAsync(x => x.Email == _request.Email);
        _userAfter.UpdatedAt.Should().BeCloseTo(_userBefore.UpdatedAt, TimeSpan.FromSeconds(1));
        _userAfter.UpdatedBy.Should().Be(_userBefore.UpdatedBy);
        _userAfter.Active.Should().Be(_userBefore.Active);
    }
}