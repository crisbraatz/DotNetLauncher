using Domain.Entities;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Presentation.Helpers;

namespace Presentation.Controllers;

/// <response code="401">Unauthorized access.</response>
/// <response code="500">An internal server error has occurred.</response>
[ApiController]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Route("api/[controller]")]
public class BaseController(IBaseEntityRepository<User> repository) : ControllerBase
{
    protected string GetClaimFromAuthorization() =>
        string.IsNullOrWhiteSpace(Request.Headers[HeaderNames.Authorization])
            ? null
            : TokenHelper.GetClaimFrom(Request.Headers[HeaderNames.Authorization]);

    protected async Task ValidateJwtAsync(CancellationToken token)
    {
        if (!await repository.ExistsByAsync(x => x.Email == GetClaimFromAuthorization() && x.Active, token))
            throw new UnauthorizedAccessException("Unauthorized access.");
    }
}