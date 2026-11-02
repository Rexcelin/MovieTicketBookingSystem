
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.DTOs.Shows;

public class CreateShowRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int MovieId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int ScreenId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal TicketPrice { get; set; }
}
