
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<int> CreateAsync(
        Booking booking,
        List<ShowSeat> showSeats);

    Task<Booking?> GetByIdAsync(int bookingId);

    Task<List<Booking>> GetAllAsync();

    Task CancelAsync(
        Booking booking,
        List<ShowSeat> showSeats);
}
