
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Auth;
using MovieTicketBookingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    // POST: api/auth/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokensAsync(request);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Invalid, expired, or revoked refresh token."
            });
        }

        return Ok(new
        {
            message = "Token refreshed successfully.",
            tokens = response
        });
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    [FromBody] LogoutRequest request)
    {
        var result = await _authService.LogoutAsync(request);

        if (!result)
        {
            return Unauthorized(new
            {
                message = "Invalid or already revoked refresh token."
            });
        }

        return Ok(new
        {
            message = "Logout successful. Refresh token has been revoked."
        });
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var fullName = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue("email")
                    ?? User.FindFirstValue(ClaimTypes.Email);

        var roles = User.FindAll(ClaimTypes.Role)
                        .Select(role => role.Value)
                        .ToList();

        return Ok(new
        {
            userId,
            fullName,
            email,
            roles
        });
    }
}
