using Application.Exceptions;
using Application.Services.Users.Helpers;
using FluentAssertions;

namespace Unit.Application.Services.Users.Helpers;

public class PasswordHelperTests
{
    private const string User = "example@template.com";
    private const string Password = "Example123";
    private Action _action;

    [Fact]
    public void Should_get_hashed_password()
    {
        var hashedPassword = User.GetHashedPassword(Password);

        hashedPassword.Should().NotBe(Password);
    }

    [Theory]
    [InlineData(Password, true)]
    [InlineData("Example1234", false)]
    public void Should_assert_match(string providedPassword, bool expectedMatch)
    {
        var returnedMatch = User.IsMatch(User.GetHashedPassword(Password), providedPassword);

        returnedMatch.Should().Be(expectedMatch);
    }

    [Fact]
    public void Should_validate_password_format()
    {
        _action = () => PasswordHelper.ValidateFormat(Password);

        _action.Should().NotThrow<Exception>();
    }

    [Theory]
    [InlineData("Example")]
    [InlineData("Example123Example123")]
    [InlineData("example123")]
    [InlineData("EXAMPLE123")]
    [InlineData("ExampleE")]
    [InlineData("12312312")]
    [InlineData("Example123!")]
    public void Should_throw_exception_validating_invalid_password_format(string password)
    {
        _action = () => PasswordHelper.ValidateFormat(password);

        _action.Should().Throw<InvalidPropertyFormatException>().WithMessage("Invalid password format.");
    }
}