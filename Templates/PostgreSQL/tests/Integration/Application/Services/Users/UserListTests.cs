using Application.Services.Users;
using Application.Services.Users.Helpers;
using Domain.DTOs.Users;
using Domain.Entities;
using Domain.Entities.Users;
using FluentAssertions;

namespace Integration.Application.Services.Users;

public class UserListTests : IntegrationBase
{
    private readonly IBaseEntityRepository<User> _repository;
    private readonly IUserList _list;
    private readonly IList<User> _users;
    private const string CustomRequestedBy = "example1@template.com";
    private ListUsersResponseDto _listResponse;

    public UserListTests()
    {
        _users = new List<User>
        {
            new("example1@template.com", "example1@template.com".GetHashedPassword("Example123")),
            new("example2@template.com", "example2@template.com".GetHashedPassword("Example123"))
        };
        _users.Last().Deactivate(_users.Last().Email);
        _repository = GetRequiredService<IBaseEntityRepository<User>>();
        _list = GetRequiredService<IUserList>();
    }

    [Fact]
    public async Task Should_list_user()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { Id = _users.First().Id });

        _listResponse.Data.Should().HaveCount(1);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(1);
        var userDto = _listResponse.Data.First();
        var userDb = _users.First();
        userDto.Email.Should().Be(userDb.Email);
        userDto.Id.Should().Be(userDb.Id);
        userDto.CreatedAt.Should().BeCloseTo(userDb.CreatedAt, TimeSpan.FromSeconds(1));
        userDto.CreatedBy.Should().Be(userDb.CreatedBy);
        userDto.UpdatedAt.Should().BeCloseTo(userDb.UpdatedAt, TimeSpan.FromSeconds(1));
        userDto.UpdatedBy.Should().Be(userDb.UpdatedBy);
        userDto.Active.Should().Be(userDb.Active);
    }

    [Fact]
    public async Task Should_return_empty_list_when_id_not_found()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { Id = Guid.NewGuid() });

        _listResponse.Data.Should().BeNullOrEmpty();
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task Should_list_users_by_email()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { Email = "example", OrderBy = new Dictionary<string, bool> { { "email", true } } });

        _listResponse.Data.Should().HaveCount(2);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(2);
        var firstUserDto = _listResponse.Data.First();
        var firstUserDb = _users.First();
        firstUserDto.Email.Should().Be(firstUserDb.Email);
        firstUserDto.Id.Should().Be(firstUserDb.Id);
        firstUserDto.CreatedAt.Should().BeCloseTo(firstUserDb.CreatedAt, TimeSpan.FromSeconds(1));
        firstUserDto.CreatedBy.Should().Be(firstUserDb.CreatedBy);
        firstUserDto.UpdatedAt.Should().BeCloseTo(firstUserDb.UpdatedAt, TimeSpan.FromSeconds(1));
        firstUserDto.UpdatedBy.Should().Be(firstUserDb.UpdatedBy);
        firstUserDto.Active.Should().Be(firstUserDb.Active);
        var lastUserDto = _listResponse.Data.Last();
        var lastUserDb = _users.Last();
        lastUserDto.Email.Should().Be(lastUserDb.Email);
        lastUserDto.Id.Should().Be(lastUserDb.Id);
        lastUserDto.CreatedAt.Should().BeCloseTo(lastUserDb.CreatedAt, TimeSpan.FromSeconds(1));
        lastUserDto.CreatedBy.Should().Be(lastUserDb.CreatedBy);
        lastUserDto.UpdatedAt.Should().BeCloseTo(lastUserDb.UpdatedAt, TimeSpan.FromSeconds(1));
        lastUserDto.UpdatedBy.Should().Be(lastUserDb.UpdatedBy);
        lastUserDto.Active.Should().Be(lastUserDb.Active);
    }

    [Fact]
    public async Task Should_list_users_by_created_at()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { CreatedAt = _users.First().CreatedAt, OrderBy = new Dictionary<string, bool> { { "email", true } } });

        _listResponse.Data.Should().HaveCount(2);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(2);
        var firstUserDto = _listResponse.Data.First();
        var firstUserDb = _users.First();
        firstUserDto.Email.Should().Be(firstUserDb.Email);
        firstUserDto.Id.Should().Be(firstUserDb.Id);
        firstUserDto.CreatedAt.Should().BeCloseTo(firstUserDb.CreatedAt, TimeSpan.FromSeconds(1));
        firstUserDto.CreatedBy.Should().Be(firstUserDb.CreatedBy);
        firstUserDto.UpdatedAt.Should().BeCloseTo(firstUserDb.UpdatedAt, TimeSpan.FromSeconds(1));
        firstUserDto.UpdatedBy.Should().Be(firstUserDb.UpdatedBy);
        firstUserDto.Active.Should().Be(firstUserDb.Active);
        var lastUserDto = _listResponse.Data.Last();
        var lastUserDb = _users.Last();
        lastUserDto.Email.Should().Be(lastUserDb.Email);
        lastUserDto.Id.Should().Be(lastUserDb.Id);
        lastUserDto.CreatedAt.Should().BeCloseTo(lastUserDb.CreatedAt, TimeSpan.FromSeconds(1));
        lastUserDto.CreatedBy.Should().Be(lastUserDb.CreatedBy);
        lastUserDto.UpdatedAt.Should().BeCloseTo(lastUserDb.UpdatedAt, TimeSpan.FromSeconds(1));
        lastUserDto.UpdatedBy.Should().Be(lastUserDb.UpdatedBy);
        lastUserDto.Active.Should().Be(lastUserDb.Active);
    }

    [Fact]
    public async Task Should_list_users_by_created_by()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { CreatedBy = _users.First().CreatedBy });

        _listResponse.Data.Should().HaveCount(1);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(1);
        var userDto = _listResponse.Data.First();
        var userDb = _users.First();
        userDto.Email.Should().Be(userDb.Email);
        userDto.Id.Should().Be(userDb.Id);
        userDto.CreatedAt.Should().BeCloseTo(userDb.CreatedAt, TimeSpan.FromSeconds(1));
        userDto.CreatedBy.Should().Be(userDb.CreatedBy);
        userDto.UpdatedAt.Should().BeCloseTo(userDb.UpdatedAt, TimeSpan.FromSeconds(1));
        userDto.UpdatedBy.Should().Be(userDb.UpdatedBy);
        userDto.Active.Should().Be(userDb.Active);
    }

    [Fact]
    public async Task Should_list_users_by_last_updated_at()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { UpdatedAt = _users.First().UpdatedAt, OrderBy = new Dictionary<string, bool> { { "email", true } } });

        _listResponse.Data.Should().HaveCount(2);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(2);
        var firstUserDto = _listResponse.Data.First();
        var firstUserDb = _users.First();
        firstUserDto.Email.Should().Be(firstUserDb.Email);
        firstUserDto.Id.Should().Be(firstUserDb.Id);
        firstUserDto.CreatedAt.Should().BeCloseTo(firstUserDb.CreatedAt, TimeSpan.FromSeconds(1));
        firstUserDto.CreatedBy.Should().Be(firstUserDb.CreatedBy);
        firstUserDto.UpdatedAt.Should().BeCloseTo(firstUserDb.UpdatedAt, TimeSpan.FromSeconds(1));
        firstUserDto.UpdatedBy.Should().Be(firstUserDb.UpdatedBy);
        firstUserDto.Active.Should().Be(firstUserDb.Active);
        var lastUserDto = _listResponse.Data.Last();
        var lastUserDb = _users.Last();
        lastUserDto.Email.Should().Be(lastUserDb.Email);
        lastUserDto.Id.Should().Be(lastUserDb.Id);
        lastUserDto.CreatedAt.Should().BeCloseTo(lastUserDb.CreatedAt, TimeSpan.FromSeconds(1));
        lastUserDto.CreatedBy.Should().Be(lastUserDb.CreatedBy);
        lastUserDto.UpdatedAt.Should().BeCloseTo(lastUserDb.UpdatedAt, TimeSpan.FromSeconds(1));
        lastUserDto.UpdatedBy.Should().Be(lastUserDb.UpdatedBy);
        lastUserDto.Active.Should().Be(lastUserDb.Active);
    }

    [Fact]
    public async Task Should_list_users_by_last_updated_by()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token)
            { UpdatedBy = _users.First().UpdatedBy });

        _listResponse.Data.Should().HaveCount(1);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(1);
        var userDto = _listResponse.Data.First();
        var userDb = _users.First();
        userDto.Email.Should().Be(userDb.Email);
        userDto.Id.Should().Be(userDb.Id);
        userDto.CreatedAt.Should().BeCloseTo(userDb.CreatedAt, TimeSpan.FromSeconds(1));
        userDto.CreatedBy.Should().Be(userDb.CreatedBy);
        userDto.UpdatedAt.Should().BeCloseTo(userDb.UpdatedAt, TimeSpan.FromSeconds(1));
        userDto.UpdatedBy.Should().Be(userDb.UpdatedBy);
        userDto.Active.Should().Be(userDb.Active);
    }

    [Fact]
    public async Task Should_list_users_by_active()
    {
        await _repository.InsertManyAsync(_users);
        await CommitAsync();

        _listResponse = await _list.ListByAsync(new ListUsersRequestDto(CustomRequestedBy, Token) { Active = true });

        _listResponse.Data.Should().HaveCount(1);
        _listResponse.CurrentPage.Should().Be(1);
        _listResponse.TotalPages.Should().Be(1);
        _listResponse.TotalItems.Should().Be(1);
        var userDto = _listResponse.Data.First();
        var userDb = _users.First();
        userDto.Email.Should().Be(userDb.Email);
        userDto.Id.Should().Be(userDb.Id);
        userDto.CreatedAt.Should().BeCloseTo(userDb.CreatedAt, TimeSpan.FromSeconds(1));
        userDto.CreatedBy.Should().Be(userDb.CreatedBy);
        userDto.UpdatedAt.Should().BeCloseTo(userDb.UpdatedAt, TimeSpan.FromSeconds(1));
        userDto.UpdatedBy.Should().Be(userDb.UpdatedBy);
        userDto.Active.Should().Be(userDb.Active);
    }
}