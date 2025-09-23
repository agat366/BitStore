namespace BitStore.Server.Controllers.Models;

/// <summary>
/// Represents a login response dto containing the JWT token.
/// </summary>
public record LoginResponse(string Token);