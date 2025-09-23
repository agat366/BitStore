using System.Security.Claims;

namespace BitStore.Server.Context;

/// <summary>
/// Provides access to current user's claims from HTTP context
/// </summary>
/// <remarks>
/// Implementation of IUserContext that extracts user information from HTTP context.
/// </remarks>
public class UserContext : IUserContext
{
    /// <summary>
    /// Extracts user identity details from claims.
    /// </summary>
    /// <param name="accessor"></param>
    public UserContext(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;
        Username = user?.Identity?.Name
                   ?? user?.FindFirst("name")?.Value;
        UserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public string Username { get; }
    public string UserId { get; }
}