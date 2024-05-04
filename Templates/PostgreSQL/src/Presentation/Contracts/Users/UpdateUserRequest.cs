using System.ComponentModel.DataAnnotations;
using Domain.DTOs.Users;

namespace Presentation.Contracts.Users;

/// <summary>
/// Update user request.
/// </summary>
public record UpdateUserRequest
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
    public string OldPassword { get; init; }

    /// <summary>
    /// Valid format password.
    /// At least one lower case letter, one upper case letter and one number.
    /// Must have between 8 and 16 characters.
    /// Must be different from old password.
    /// </summary>
    [Required]
    public string NewPassword { get; init; }

    public UpdateUserRequestDto ConvertToDto(string requestedBy, CancellationToken token) =>
        new(Email, OldPassword, NewPassword, requestedBy, token);
}