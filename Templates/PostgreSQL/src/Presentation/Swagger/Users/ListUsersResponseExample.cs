using Presentation.Contracts.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Swagger.Users;

public class ListUsersResponseExample : IExamplesProvider<ListUsersResponse>
{
    public ListUsersResponse GetExamples() => new()
    {
        Data = new List<ListUserResponse> { new ListUserResponseExample().GetExamples() },
        CurrentPage = 1,
        TotalPages = 10,
        TotalItems = 100
    };
}