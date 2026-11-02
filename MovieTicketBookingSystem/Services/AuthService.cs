
using Microsoft.AspNetCore.Identity;
using MovieTicketBookingSystem.DTOs.Auth;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    // CUSTOMER REGISTRATION
    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var email = request.Email.Trim();

        // Check whether email already exists
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        // Create new customer
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim()
        };

        // Identity securely hashes the password
        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        // Assign Customer role
        var roleResult = await _userManager.AddToRoleAsync(
            user,
            "Customer");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Failed to assign Customer role: {errors}");
        }

        // Generate access and refresh tokens
        var tokens = await _tokenService.GenerateTokensAsync(user);

        return new AuthResponse
        {
            Message = "Registration successful.",
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Tokens = tokens
        };
    }

    // CUSTOMER / ADMIN LOGIN
    public async Task<AuthResponse> LoginAsync(
        LoginRequest request)
    {
        var email = request.Email.Trim();

        // Find user by email
        var user = await _userManager.FindByEmailAsync(email);

        // Validate credentials
        if (user is null ||
            !await _userManager.CheckPasswordAsync(
                user,
                request.Password))
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        // Check assigned roles
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Count == 0)
        {
            throw new UnauthorizedAccessException(
                "Your account does not have an assigned role. Contact the administrator.");
        }

        // Generate access and refresh tokens
        var tokens = await _tokenService.GenerateTokensAsync(user);

        return new AuthResponse
        {
            Message = "Login successful.",
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Tokens = tokens
        };
    }

    // REFRESH ACCESS TOKEN
    public async Task<TokenResponse?> RefreshTokensAsync(
        RefreshTokenRequest request)
    {
        // Validate incoming refresh token
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return null;
        }

        // Delegate validation and rotation to TokenService
        return await _tokenService.RefreshTokensAsync(
            request.RefreshToken);
    }

    public async Task<bool> LogoutAsync(LogoutRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return false;
        }

        return await _tokenService.RevokeRefreshTokenAsync(
            request.RefreshToken);
    }
}
