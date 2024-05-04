using System.Text.RegularExpressions;
using Application.Exceptions;

namespace Application.Services.Users.Helpers;

public static partial class EmailHelper
{
    public static void ValidateFormat(string email)
    {
        if (!Regex().IsMatch(email))
            throw new InvalidPropertyFormatException("Invalid email format.");
    }

    [GeneratedRegex(@"^[\w!#$%&'*+/=?`{|}~^-]+(?:\.[\w!#$%&'*+/=?`{|}~^-]+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$")]
    private static partial Regex Regex();
}