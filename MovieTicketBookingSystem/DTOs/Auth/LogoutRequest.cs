using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Auth;

public class LogoutRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}