namespace BitStore.Server.Services;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface IAuthService
{
    string GenerateToken(string username);
    bool ValidateToken(string token);
}