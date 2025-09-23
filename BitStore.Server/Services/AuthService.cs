using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BitStore.Core.Services;
using Microsoft.IdentityModel.Tokens;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BitStore.Server.Services;

/// <summary>
/// Handles user authentication and JWT token management.
/// </summary>
public class AuthService : IAuthService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IDataService _dataService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IConfiguration configuration,
        IDataService dataService,
        ILogger<AuthService> logger)
    {
        _secretKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key must be configured");
        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer must be configured");
        _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience must be configured");
        _dataService = dataService;
        _logger = logger;
    }

    public async Task<string> GenerateToken(string username)
    {
        try
        {
            // Try to get existing user or create new one
            var user = await _dataService.GetUserByLoginAsync(username);
            if (user == null)
            {
                _logger.LogInformation("Creating new user: {Username}", username);
                user = await _dataService.CreateUserAsync(username);
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Store the token with the user
            await _dataService.UpdateUserTokenAsync(user.Id, tokenString);

            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate token for user: {Username}", username);
            throw;
        }
    }

    public async Task<bool> ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }
}