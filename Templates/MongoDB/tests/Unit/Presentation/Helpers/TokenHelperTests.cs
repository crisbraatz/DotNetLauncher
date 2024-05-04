using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Presentation.Helpers;

namespace Unit.Presentation.Helpers;

public class TokenHelperTests
{
    [Fact]
    public void Should_generate_jwt_for_email()
    {
        var jwt = TokenHelper.GenerateJwtFor("example@template.com");

        jwt.Should().HaveLength(267);
    }

    [Fact]
    public void Should_get_claim_from_authorization()
    {
        StringValues authorization =
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImV4YW1wbGVAdGVtcGxhdGUuY29tIiwibmJmIjoxNjI4MzUzMjQ3LCJleHAiOjE2MjgzNTY4NDcsImlhdCI6MTYyODM1MzI0NywiaXNzIjoiREVGQVVMVEpXVElTU1VFUiIsImF1ZCI6IkRFRkFVTFRKV1RBVURJRU5DRSJ9.zrokqq5IZoSwZLAl9AoNRlhGn1haKuvm_o4urzP08w4";

        var claim = TokenHelper.GetClaimFrom(authorization);

        claim.Should().Be("example@template.com");
    }

    [Fact]
    public void Should_get_security_key()
    {
        Environment.SetEnvironmentVariable("JWT_SECURITY_KEY", "NOT_DEFAULT_256_BITS_JWT_SECURITY_KEY");

        var securityKey = TokenHelper.GetSecurityKey();

        securityKey.KeySize.Should().Be(296);
    }
}