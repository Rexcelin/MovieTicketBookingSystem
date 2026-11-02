using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class Screen
{
    public int ScreenId { get; set; }

    [Required]
    public int TheatreId { get; set; }

    [Required]
    [StringLength(100)]
    public string ScreenName { get; set; } = string.Empty;

    // Navigation property
    public Theatre Theatre { get; set; } = null!;

    public ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public ICollection<Show> Shows { get; set; } = new List<Show>();
}