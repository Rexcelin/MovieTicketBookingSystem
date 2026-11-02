using MovieTicketBookingSystem.DTOs.Auth;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokensAsync(ApplicationUser user);

    Task<TokenResponse?> RefreshTokensAsync(string refreshToken);

    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}