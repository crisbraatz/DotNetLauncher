using Application.Services.Users;
using Domain.Entities;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Users;
using Presentation.Helpers;

namespace Presentation.Controllers;

public class UsersController(
    IBaseEntityRepository<User> repository,
    IUserAuthenticator authenticator,
    IUserCreator creator,
    IUserDeactivator deactivator,
    IUserList list,
    IUserPasswordUpdater passwordUpdater)
    : BaseController(repository)
{
    /// <summary>
    /// Authenticates the user using the email and password provided.
    /// </summary>
    /// <response code="200">The authorization token.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="404">User not found.</response>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AuthenticateAsync(
        [FromBody] AuthenticateUserRequest request, CancellationToken token = default)
    {
        var email = GetClaimFromAuthorization();

        await authenticator.AuthenticateAsync(request.ConvertToDto(email, token));

        return Ok($"Bearer {TokenHelper.GenerateJwtFor(request.Email.ToLowerInvariant())}");
    }

    /// <summary>
    /// Creates the user using the email and password provided.
    /// </summary>
    /// <response code="200">User created.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="409">User already created.</response>
    [AllowAnonymous]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateUserRequest request, CancellationToken token = default)
    {
        await creator.CreateAsync(request.ConvertToDto(GetClaimFromAuthorization(), token));

        return Ok($"Created user {request.Email}.");
    }

    /// <summary>
    /// Deactivates the user using the email and password provided.
    /// </summary>
    /// <response code="200">User deactivated.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAsync(
        [FromBody] DeactivateUserRequest request, CancellationToken token = default)
    {
        await ValidateJwtAsync(token);

        await deactivator.DeactivateAsync(request.ConvertToDto(GetClaimFromAuthorization(), token));

        return Ok($"Deactivated user {request.Email}.");
    }

    /// <summary>
    /// Lists the users using the filters provided.
    /// </summary>
    /// <response code="200">Users listed.</response>
    [HttpGet("listBy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ListUsersResponse>> ListByAsync(
        [FromQuery] ListUsersRequest request, CancellationToken token = default)
    {
        await ValidateJwtAsync(token);

        return Ok(ListUsersResponse.ConvertFromDto(
            await list.ListByAsync(request.ConvertToDto(GetClaimFromAuthorization(), token))));
    }

    /// <summary>
    /// Updates the user's password using the email, old and new passwords provided.
    /// </summary>
    /// <response code="200">User password updated.</response>
    /// <response code="400">The request was unsuccessful, see details.</response>
    /// <response code="404">User not found.</response>
    [HttpPatch("updatePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePasswordAsync(
        [FromBody] UpdateUserRequest request, CancellationToken token = default)
    {
        await ValidateJwtAsync(token);

        await passwordUpdater.UpdatePasswordAsync(request.ConvertToDto(GetClaimFromAuthorization(), token));

        return Ok($"Updated user {request.Email} password.");
    }
}