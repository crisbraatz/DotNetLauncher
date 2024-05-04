using Application.Exceptions;
using Application.Services.Users;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using FluentAssertions;

namespace Integration.Application.Services.Users;

public class UserPasswordUpdaterTests : IntegrationBase
{
    private readonly IBaseEntityRepository<User> _repository;
    private readonly IUserPasswordUpdater _passwordUpdater;

    private readonly User _userBefore =
        new("example@template.com", "example@template.com".GetHashedPassword("OldExample123"));

    private User _userAfter;
    private UpdateUserRequestDto _request;
    private Func<Task> _func;

    private UpdateUserRequestDto BuildRequest(
        string email = "example@template.com",
        string oldPassword = "OldExample123",
        string newPassword = "NewExample123",
        string requestedBy = RequestedBy) =>
        new(email, oldPassword, newPassword, requestedBy, Token);

    public UserPasswordUpdaterTests()
    {
        _repository = GetRequiredService<IBaseEntityRepository<User>>();
        _passwordUpdater = GetRequiredService<IUserPasswordUpdater>();
    }

    [Fact]
    public async Task Should_update_user_password()
    {
        await _repository.InsertOneAsync(_userBefore);
        await CommitAsync();
        _request = BuildRequest();

        await _passwordUpdater.UpdatePasswordAsync(_request);
        await CommitAsync();

        _userAfter = await _repository.SelectOneByAsync(x => x.Email == _request.Email);
        _userAfter.Password.Should().NotBe(_request.Email.GetHashedPassword(_request.OldPassword));
        _userAfter.UpdatedAt.Should().BeAfter(_userAfter.CreatedAt);
        _userAfter.UpdatedBy.Should().Be(RequestedBy);
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_email_format()
    {
        _func = () => _passwordUpdater.UpdatePasswordAsync(
            BuildRequest(email: "example@template.com.", requestedBy: "example@template.com."));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_throw_exception_if_authenticated_user_different_from_request_user()
    {
        _request = BuildRequest(requestedBy: "another.example@template.com");

        _func = () => _passwordUpdater.UpdatePasswordAsync(_request);

        await _func.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage(
            $"Authenticated user {_request.RequestedBy} different from request user {_request.Email}.");
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_old_password_format()
    {
        _func = () => _passwordUpdater.UpdatePasswordAsync(BuildRequest(oldPassword: "Example"));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid password format.");
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_new_password_format()
    {
        _func = () => _passwordUpdater.UpdatePasswordAsync(BuildRequest(newPassword: "Example"));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid password format.");
    }

    [Fact]
    public async Task Should_throw_exception_if_old_and_new_password_are_equal()
    {
        _request = BuildRequest(oldPassword: "Example123", newPassword: "Example123");

        _func = () => _passwordUpdater.UpdatePasswordAsync(_request);

        await _func.Should().ThrowAsync<InvalidPropertyValueException>()
            .WithMessage($"Old and new password are equal for user {_request.Email}.");
    }

    [Fact]
    public async Task Should_throw_exception_if_user_not_found()
    {
        _request = BuildRequest();

        _func = () => _passwordUpdater.UpdatePasswordAsync(_request);

        await _func.Should().ThrowAsync<EntityNotFoundException>().WithMessage($"User {_request.Email} not found.");
    }

    [Fact]
    public async Task Should_throw_exception_if_invalid_old_password()
    {
        await _repository.InsertOneAsync(_userBefore);
        await CommitAsync();
        _request = BuildRequest(oldPassword: "Example12");

        _func = () => _passwordUpdater.UpdatePasswordAsync(_request);
        await CommitAsync();

        await _func.Should().ThrowAsync<InvalidPropertyValueException>()
            .WithMessage($"Invalid old password for user {_request.Email}.");
        _userAfter = await _repository.SelectOneByAsync(x => x.Email == _request.Email);
        _userAfter.Password.Should().Be(_userBefore.Password);
        _userAfter.UpdatedAt.Should().BeCloseTo(_userBefore.UpdatedAt, TimeSpan.FromSeconds(1));
        _userAfter.UpdatedBy.Should().Be(_userBefore.UpdatedBy);
    }
}