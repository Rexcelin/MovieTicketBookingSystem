using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface ISeatRepository
{
    Task<Seat> AddAsync(Seat seat);

    Task AddRangeAsync(IEnumerable<Seat> seats);

    Task<Seat?> GetByIdAsync(int seatId);

    Task<List<Seat>> GetAllAsync();

    Task<List<Seat>> GetByScreenIdAsync(int screenId);

    Task<bool> ExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null);

    Task UpdateAsync(Seat seat);

    Task<bool> DeleteAsync(int seatId);
}