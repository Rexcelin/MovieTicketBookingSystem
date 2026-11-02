
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using MovieTicketBookingSystem.Configurations;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.DTOs.Auth;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TokenService(
        IOptions<JwtSettings> jwtOptions,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
        _context = context;
    }

    // GENERATE ACCESS TOKEN AND REFRESH TOKEN
    public async Task<TokenResponse> GenerateTokensAsync(
        ApplicationUser user)
    {
        var now = DateTime.UtcNow;

        var accessTokenExpiresAt = now.AddMinutes(
            _jwtSettings.AccessTokenExpirationMinutes);

        var refreshTokenExpiresAt = now.AddDays(
            _jwtSettings.RefreshTokenExpirationDays);

        // Get user's assigned roles
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // Generate cryptographically secure refresh token
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        var refreshToken = Convert.ToBase64String(randomBytes);

        // Store only the SHA-256 hash
        var refreshTokenHash = HashRefreshToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenHash,
            CreatedAt = now,
            ExpiresAt = refreshTokenExpiresAt
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }

    // REFRESH TOKEN ROTATION
    public async Task<TokenResponse?> RefreshTokensAsync(
        string refreshToken)
    {
        var tokenHash = HashRefreshToken(refreshToken);

        // Find the stored token and its associated user
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == tokenHash);

        // Validate the refresh token
        if (storedToken is null ||
            storedToken.IsRevoked ||
            storedToken.IsExpired)
        {
            return null;
        }

        // Begin database transaction
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Revoke the old refresh token
            storedToken.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Generate a new access token and refresh token
            var newTokens = await GenerateTokensAsync(
                storedToken.User);

            // Commit both changes together
            await transaction.CommitAsync();

            return newTokens;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // HASH REFRESH TOKEN USING SHA-256
    private static string HashRefreshToken(string refreshToken)
    {
        return Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshToken)));
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        string hashedToken = HashRefreshToken(refreshToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

        if (storedToken is null || storedToken.IsRevoked)
        {
            return false;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}
