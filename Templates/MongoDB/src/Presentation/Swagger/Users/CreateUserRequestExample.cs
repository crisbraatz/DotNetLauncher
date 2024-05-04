using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class CreateUserRequestExample : IExamplesProvider<CreateUserRequest>
{
    public CreateUserRequest GetExamples() => new()
    {
        Email = "example@template.com",
        Password = "Example123"
    };
}