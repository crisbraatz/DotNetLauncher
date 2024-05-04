using System.Text.RegularExpressions;
using Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Users.Helpers;

public static partial class PasswordHelper
{
    private static readonly PasswordHasher<string> PasswordHasher = new();

    public static string GetHashedPassword(this string user, string password) =>
        PasswordHasher.HashPassword(user, password);

    public static bool IsMatch(this string user, string hashedPassword, string providedPassword) =>
        PasswordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword)
            is not PasswordVerificationResult.Failed;

    public static void ValidateFormat(string password)
    {
        if (!Regex().IsMatch(password))
            throw new InvalidPropertyFormatException("Invalid password format.");
    }

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,16}$")]
    private static partial Regex Regex();
}