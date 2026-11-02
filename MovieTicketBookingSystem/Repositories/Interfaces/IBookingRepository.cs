using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<int> CreateAsync(
        Booking booking,
        List<ShowSeat> showSeats);

    Task<Booking?> GetByIdAsync(int bookingId);

    Task<Booking?> GetByIdForUserAsync(
        int bookingId,
        string userId);

    Task<List<Booking>> GetAllAsync();

    Task<List<Booking>> GetByUserIdAsync(string userId);

    Task CancelAsync(
        Booking booking,
        List<ShowSeat> showSeats);
}