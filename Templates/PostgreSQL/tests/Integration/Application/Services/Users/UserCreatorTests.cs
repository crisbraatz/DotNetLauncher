using Application.Exceptions;
using Application.Services.Users;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using FluentAssertions;

namespace Integration.Application.Services.Users;

public class UserCreatorTests : IntegrationBase
{
    private readonly IBaseEntityRepository<User> _repository;
    private readonly IUserCreator _creator;
    private User _user;
    private CreateUserRequestDto _request;
    private Func<Task> _func;

    private CreateUserRequestDto BuildRequest(
        string email = "example@template.com", string password = "Example123", string requestedBy = null) =>
        new(email, password, requestedBy, Token);

    public UserCreatorTests()
    {
        _repository = GetRequiredService<IBaseEntityRepository<User>>();
        _creator = GetRequiredService<IUserCreator>();
    }

    [Fact]
    public async Task Should_create_user()
    {
        _request = BuildRequest();

        await _creator.CreateAsync(_request);
        await CommitAsync();

        _user = await _repository.SelectOneByAsync(x => x.Email == _request.Email);
        _user.Email.Should().Be(_request.Email);
        _user.Password.Should().NotBe(_request.Password).And.NotBeNullOrWhiteSpace();
        _user.Id.Should().NotBeEmpty();
        _user.CreatedBy.Should().Be(_request.Email);
        _user.UpdatedBy.Should().Be(_request.Email);
        _user.Active.Should().BeTrue();
    }

    [Fact]
    public async Task Should_throw_exception_when_authenticated_user_request_creation()
    {
        _request = BuildRequest(email: "another.example@template.com", requestedBy: RequestedBy);

        _func = () => _creator.CreateAsync(_request);

        await _func.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage($"Authenticated user {_request.RequestedBy} can not request user creation.");
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_email_format()
    {
        _func = () => _creator.CreateAsync(BuildRequest(email: "example@template.com."));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_throw_exception_when_invalid_password_format()
    {
        _func = () => _creator.CreateAsync(BuildRequest(password: "Example"));

        await _func.Should().ThrowAsync<InvalidPropertyFormatException>().WithMessage("Invalid password format.");
    }

    [Fact]
    public async Task Should_throw_exception_when_user_already_created()
    {
        await _repository.InsertOneAsync(
            new User("example@template.com", "example@template.com".GetHashedPassword("Example123")));
        await CommitAsync();
        _request = BuildRequest();

        _func = () => _creator.CreateAsync(_request);
        await CommitAsync();

        await _func.Should().ThrowAsync<EntityAlreadyExistsException>()
            .WithMessage($"User {_request.Email} already created.");
        _user = await _repository.SelectOneByAsync(x => x.Email == _request.Email);
        _user.Should().NotBeNull();
    }
}