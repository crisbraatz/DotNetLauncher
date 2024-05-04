using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class ListUsersRequestExample : IExamplesProvider<ListUsersRequest>
{
    public ListUsersRequest GetExamples() => new()
    {
        Email = "example@template.com",
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow.AddHours(-1),
        CreatedBy = "example@template.com",
        UpdatedAt = DateTime.UtcNow,
        UpdatedBy = "example@template.com",
        Active = true,
        OrderBy = "id asc",
        Page = 1,
        Size = 10
    };
}