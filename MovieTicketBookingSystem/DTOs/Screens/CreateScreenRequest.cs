using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Screens;

public class CreateScreenRequest
{
    [Required]
    public int TheatreId { get; set; }

    [Required]
    [StringLength(100)]
    public string ScreenName { get; set; } = string.Empty;
}