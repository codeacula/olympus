using System.Security.Claims;

namespace Olympus.Application.Abstractions;

/// <summary>
/// Provides information about the current user context.
/// </summary>
public interface ICurrentUserProvider
{
  /// <summary>
  /// Gets the current user's unique identifier, or null if unauthenticated.
  /// </summary>
  string? UserId { get; }

  /// <summary>
  /// Gets the current user's claims principal.
  /// </summary>
  ClaimsPrincipal? Principal { get; }
}
