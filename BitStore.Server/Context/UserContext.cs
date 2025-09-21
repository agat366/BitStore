namespace BitStore.Server.Context;

/// <summary>
/// Provides access to current user's claims from HTTP context
/// </summary>
public class UserContext : IUserContext
{
    /// <summary>
    /// Extracts username and user ID from claims
    /// </summary>
    /// <param name="accessor"></param>
    public UserContext(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User;
        Username = user?.Identity?.Name
                   ?? user?.FindFirst("name")?.Value;
        UserId = user?.FindFirst("sub")?.Value;
    }

    public string Username { get; }
    public string UserId { get; }
}