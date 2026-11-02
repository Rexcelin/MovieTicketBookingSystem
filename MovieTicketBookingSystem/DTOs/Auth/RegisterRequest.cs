
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
