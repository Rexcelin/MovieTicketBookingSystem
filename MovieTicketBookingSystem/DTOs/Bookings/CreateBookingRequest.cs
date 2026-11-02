
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Bookings;

public class CreateBookingRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ShowId { get; set; }

    [Required]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(15)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<int> SeatIds { get; set; } = new();
}
