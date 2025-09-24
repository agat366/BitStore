using System.Security.Claims;

namespace BitStore.Server.Context;

/// <inheritdoc cref="IUserContext" />
public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name 
        ?? throw new InvalidOperationException("User not authenticated");

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new InvalidOperationException("User ID not found in claims");
}