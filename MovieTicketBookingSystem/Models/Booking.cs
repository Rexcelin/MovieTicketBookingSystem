using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class Booking
{
    public int BookingId { get; set; }

    [Required]
    [StringLength(30)]
    public string BookingReference { get; set; } = string.Empty;

    [Required]
    public int ShowId { get; set; }

    // Authenticated user who created this booking
    [Required]
    public string UserId { get; set; } = string.Empty;

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

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Confirmed";

    // Navigation properties
    public ApplicationUser User { get; set; } = null!;

    public Show Show { get; set; } = null!;

    public ICollection<BookingSeat> BookingSeats { get; set; }
        = new List<BookingSeat>();
}