using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BitStore.Core.Services;
using Microsoft.IdentityModel.Tokens;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BitStore.Server.Services;

/// <inheritdoc cref="IAuthService" />
public class AuthService(
    IConfiguration configuration,
    IDataService dataService,
    ILogger<AuthService> logger)
    : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IDataService _dataService = dataService;
    private readonly ILogger<AuthService> _logger = logger;

    private readonly string _secretKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key must be configured");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer must be configured");
    private readonly string _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience must be configured");

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