using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class UpdateUserRequestExample : IExamplesProvider<UpdateUserRequest>
{
    public UpdateUserRequest GetExamples() => new()
    {
        Email = "example@template.com",
        OldPassword = "OldExample123",
        NewPassword = "NewExample123"
    };
}