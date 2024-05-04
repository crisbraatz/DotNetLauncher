using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class ListUserResponseExample : IExamplesProvider<ListUserResponse>
{
    public ListUserResponse GetExamples() => new()
    {
        Email = "example@template.com",
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow.AddHours(-1),
        CreatedBy = "example@template.com",
        UpdatedAt = DateTime.UtcNow,
        UpdatedBy = "example@template.com",
        Active = true
    };
}