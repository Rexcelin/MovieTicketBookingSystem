using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class Seat
{
    public int SeatId { get; set; }

    [Required]
    public int ScreenId { get; set; }

    [Required]
    [StringLength(5)]
    public string RowName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int SeatNumber { get; set; }

    [Required]
    [StringLength(30)]
    public string SeatType { get; set; } = string.Empty;

    // Navigation properties
    public Screen Screen { get; set; } = null!;

    public ICollection<ShowSeat> ShowSeats { get; set; }
        = new List<ShowSeat>();

    public ICollection<BookingSeat> BookingSeats { get; set; }
        = new List<BookingSeat>();
}