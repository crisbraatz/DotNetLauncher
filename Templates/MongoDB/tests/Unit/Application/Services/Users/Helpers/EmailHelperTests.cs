using Application.Exceptions;
using Application.Services.Users.Helpers;
using FluentAssertions;

namespace Unit.Application.Services.Users.Helpers;

public class EmailHelperTests
{
    private Action _action;

    [Theory]
    [InlineData("example@template.com")]
    [InlineData("example@template.com.br")]
    [InlineData("example@template.com.br.sc")]
    [InlineData("example.example@template.com")]
    [InlineData("example_example@template.com")]
    public void Should_validate_email_format(string email)
    {
        _action = () => EmailHelper.ValidateFormat(email);

        _action.Should().NotThrow<Exception>();
    }

    [Theory]
    [InlineData(".example@template.com")]
    [InlineData("example@template.com.")]
    [InlineData("example@template..com")]
    [InlineData("example@template.c")]
    [InlineData("example@template.company")]
    public void Should_throw_exception_validating_invalid_email_format(string email)
    {
        _action = () => EmailHelper.ValidateFormat(email);

        _action.Should().Throw<InvalidPropertyFormatException>().WithMessage("Invalid email format.");
    }
}