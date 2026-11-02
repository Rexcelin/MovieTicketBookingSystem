using MovieTicketBookingSystem.DTOs.Auth;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<TokenResponse?> RefreshTokensAsync(
        RefreshTokenRequest request);

    Task<bool> LogoutAsync(LogoutRequest request);
}