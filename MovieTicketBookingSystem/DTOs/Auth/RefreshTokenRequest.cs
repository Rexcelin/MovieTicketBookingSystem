
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
