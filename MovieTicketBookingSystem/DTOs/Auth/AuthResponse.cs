
namespace MovieTicketBookingSystem.DTOs.Auth;

public class AuthResponse
{
    public string Message { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public TokenResponse? Tokens { get; set; }
}
