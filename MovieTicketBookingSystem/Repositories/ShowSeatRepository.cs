
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class ShowSeatRepository : IShowSeatRepository
{
    private readonly ApplicationDbContext _context;

    public ShowSeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShowSeat>> GetByShowIdAsync(int showId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId)
            .OrderBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();
    }

    public async Task<ShowSeat?> GetByIdAsync(int showSeatId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .FirstOrDefaultAsync(ss => ss.ShowSeatId == showSeatId);
    }

    public async Task<List<ShowSeat>> GetAvailableSeatsAsync(int showId)
    {
        return await _context.ShowSeats
            .AsNoTracking()
            .Include(ss => ss.Seat)
            .Where(ss =>
                ss.ShowId == showId &&
                ss.Status == "Available")
            .OrderBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int showId, int seatId)
    {
        return await _context.ShowSeats
            .AnyAsync(ss =>
                ss.ShowId == showId &&
                ss.SeatId == seatId);
    }
}
