using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Users;

namespace Presentation.Contracts.Users;

/// <summary>
/// Create user request.
/// </summary>
public record CreateUserRequest
{
    /// <summary>
    /// Valid format email.
    /// </summary>
    [Required]
    public string Email { get; init; }

    /// <summary>
    /// Valid format password.
    /// At least one lower case letter, one upper case letter and one number.
    /// Must have between 8 and 16 characters.
    /// </summary>
    [Required]
    public string Password { get; init; }

    public CreateUserRequestDto ConvertToDto(string requestedBy, CancellationToken token) =>
        new(Email, Password, requestedBy, token);
}