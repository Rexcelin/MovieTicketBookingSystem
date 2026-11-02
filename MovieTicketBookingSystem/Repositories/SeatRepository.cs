using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;

    public SeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add a single seat
    public async Task<Seat> AddAsync(Seat seat)
    {
        await _context.Seats.AddAsync(seat);
        await _context.SaveChangesAsync();

        return seat;
    }

    // Add multiple seats in bulk
    public async Task AddRangeAsync(IEnumerable<Seat> seats)
    {
        await _context.Seats.AddRangeAsync(seats);
        await _context.SaveChangesAsync();
    }

    // Get seat by ID
    public async Task<Seat?> GetByIdAsync(int seatId)
    {
        return await _context.Seats
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SeatId == seatId);
    }

    // Get all seats
    public async Task<List<Seat>> GetAllAsync()
    {
        return await _context.Seats
            .AsNoTracking()
            .OrderBy(s => s.ScreenId)
            .ThenBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    // Get seats by screen ID
    public async Task<List<Seat>> GetByScreenIdAsync(int screenId)
    {
        return await _context.Seats
            .AsNoTracking()
            .Where(s => s.ScreenId == screenId)
            .OrderBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    // Check duplicate seat coordinates
    public async Task<bool> ExistsAsync(
        int screenId,
        string rowName,
        int seatNumber,
        int? excludeSeatId = null)
    {
        var query = _context.Seats
            .AsQueryable()
            .Where(s =>
                s.ScreenId == screenId &&
                s.RowName == rowName &&
                s.SeatNumber == seatNumber);

        if (excludeSeatId.HasValue)
        {
            query = query.Where(s => s.SeatId != excludeSeatId.Value);
        }

        return await query.AnyAsync();
    }

    // Update seat
    public async Task UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);
        await _context.SaveChangesAsync();
    }

    // Delete seat
    public async Task<bool> DeleteAsync(int seatId)
    {
        var seat = await _context.Seats
            .FirstOrDefaultAsync(s => s.SeatId == seatId);

        if (seat == null)
        {
            return false;
        }

        _context.Seats.Remove(seat);
        await _context.SaveChangesAsync();

        return true;
    }
}