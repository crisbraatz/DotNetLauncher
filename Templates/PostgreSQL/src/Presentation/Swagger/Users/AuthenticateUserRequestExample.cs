using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class AuthenticateUserRequestExample : IExamplesProvider<AuthenticateUserRequest>
{
    public AuthenticateUserRequest GetExamples() => new()
    {
        Email = "example@template.com",
        Password = "Example123"
    };
}