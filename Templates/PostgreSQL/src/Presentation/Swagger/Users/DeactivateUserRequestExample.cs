using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class DeactivateUserRequestExample : IExamplesProvider<DeactivateUserRequest>
{
    public DeactivateUserRequest GetExamples() => new()
    {
        Email = "example@template.com",
        Password = "Example123"
    };
}