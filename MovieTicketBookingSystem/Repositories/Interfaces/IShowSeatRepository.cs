
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IShowSeatRepository
{
    Task<List<ShowSeat>> GetByShowIdAsync(int showId);

    Task<ShowSeat?> GetByIdAsync(int showSeatId);

    Task<List<ShowSeat>> GetAvailableSeatsAsync(int showId);

    Task<bool> ExistsAsync(int showId, int seatId);
}
