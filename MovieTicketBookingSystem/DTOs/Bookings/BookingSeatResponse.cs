
namespace MovieTicketBookingSystem.DTOs.Bookings;

public class BookingSeatResponse
{
    public int BookingSeatId { get; set; }

    public int SeatId { get; set; }

    public string RowName { get; set; } = string.Empty;

    public int SeatNumber { get; set; }

    public string SeatType { get; set; } = string.Empty;

    public decimal Price { get; set; }
}
