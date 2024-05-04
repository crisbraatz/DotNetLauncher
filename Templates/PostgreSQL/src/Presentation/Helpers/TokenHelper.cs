using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Helpers;

public static class TokenHelper
{
    public static string GenerateJwtFor(string email)
    {
        var handler = new JwtSecurityTokenHandler();

        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Audience = AppSettings.JwtAudience,
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = AppSettings.JwtIssuer,
            SigningCredentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new[] { new Claim(nameof(email), email) })
        });

        return handler.WriteToken(token);
    }

    public static string GetClaimFrom(StringValues authorization)
    {
        var token = new JwtSecurityTokenHandler().ReadToken(authorization.ToString().Trim()[7..]) as JwtSecurityToken;

        return token?.Claims
            .First(x => x.Type.Equals("email", StringComparison.InvariantCultureIgnoreCase)).Value
            .ToLowerInvariant();
    }

    public static SymmetricSecurityKey GetSecurityKey() => new(Encoding.ASCII.GetBytes(AppSettings.JwtSecurityKey));
}