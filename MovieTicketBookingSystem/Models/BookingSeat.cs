
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class BookingSeat
{
    public int BookingSeatId { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int SeatId { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal Price { get; set; }

    // Navigation properties
    public Booking Booking { get; set; } = null!;

    public Seat Seat { get; set; } = null!;
}
