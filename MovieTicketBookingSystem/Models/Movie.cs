
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models;

public class Movie
{
    public int MovieId { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }

    public DateTime ReleaseDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Language { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Show> Shows { get; set; } = new List<Show>();
}
