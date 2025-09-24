using BitStore.Server.Controllers.Models;
using Microsoft.AspNetCore.Mvc;
using BitStore.Server.Services;

namespace BitStore.Server.Controllers;

/// <summary>
/// Handles authentication endpoints.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {       
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token (currently using a primitive approach (with username only) just for multi-user UI purpose).
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest(new { error = "Username is required" });
        }

        try
        {
            var token = await _authService.GenerateToken(request.Username);
            _logger.LogInformation("Login successful for user: {Username}", request.Username);
            
            return Ok(new LoginResponse(token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user: {Username}", request.Username);
            return StatusCode(500, new { error = "Authentication failed" });
        }
    }
}