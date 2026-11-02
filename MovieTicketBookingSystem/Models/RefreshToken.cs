
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Token { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public ApplicationUser User { get; set; } = null!;
}
