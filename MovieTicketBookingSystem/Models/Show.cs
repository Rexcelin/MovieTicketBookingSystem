
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class Show
{
    public int ShowId { get; set; }

    [Required]
    public int MovieId { get; set; }

    [Required]
    public int ScreenId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal TicketPrice { get; set; }

    // Navigation properties
    public Movie Movie { get; set; } = null!;

    public Screen Screen { get; set; } = null!;

    public ICollection<ShowSeat> ShowSeats { get; set; } = new List<ShowSeat>();

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
