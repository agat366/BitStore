namespace BitStore.Server.Services;

/// <summary>
/// Defines authentication operations for user management and token handling.
/// </summary>
public interface IAuthService
{
    Task<string> GenerateToken(string username);
    Task<bool> ValidateToken(string token);
}