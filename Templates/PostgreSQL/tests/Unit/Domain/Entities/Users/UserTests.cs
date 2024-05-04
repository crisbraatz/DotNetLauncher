using Domain.Entities.Users;
using FluentAssertions;

namespace Unit.Domain.Entities.Users;

public class UserTests
{
    private const string Email = "example@template.com";
    private const string Password = "Example123";
    private readonly DateTime _dateTime = DateTime.UtcNow;
    private readonly User _user = new(Email, Password);

    [Fact]
    public void Should_create_an_user()
    {
        _user.Email.Should().Be(Email);
        _user.Password.Should().Be(Password);
        _user.Id.Should().NotBeEmpty();
        _user.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _user.CreatedBy.Should().Be(Email);
        _user.UpdatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _user.UpdatedBy.Should().Be(Email);
        _user.Active.Should().BeTrue();
    }

    [Fact]
    public void Should_deactivate_an_user()
    {
        Thread.Sleep(100);

        _user.Deactivate(Email);

        _user.Email.Should().Be(Email);
        _user.Password.Should().Be(Password);
        _user.Id.Should().NotBeEmpty();
        _user.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _user.CreatedBy.Should().Be(Email);
        _user.UpdatedAt.Should().BeAfter(_user.CreatedAt);
        _user.UpdatedBy.Should().Be(Email);
        _user.Active.Should().BeFalse();
    }

    [Fact]
    public void Should_update_an_user_password()
    {
        const string newPassword = "Example1234";
        Thread.Sleep(100);

        _user.UpdatePassword(newPassword, Email);

        _user.Email.Should().Be(Email);
        _user.Password.Should().Be(newPassword);
        _user.Id.Should().NotBeEmpty();
        _user.CreatedAt.Should().BeCloseTo(_dateTime, TimeSpan.FromSeconds(1));
        _user.CreatedBy.Should().Be(Email);
        _user.UpdatedAt.Should().BeAfter(_user.CreatedAt);
        _user.UpdatedBy.Should().Be(Email);
        _user.Active.Should().BeTrue();
    }
}