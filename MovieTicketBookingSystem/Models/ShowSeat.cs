
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class ShowSeat
{
    public int ShowSeatId { get; set; }

    [Required]
    public int ShowId { get; set; }

    [Required]
    public int SeatId { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Available";

    // Navigation properties
    public Show Show { get; set; } = null!;

    public Seat Seat { get; set; } = null!;
}
