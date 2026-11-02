using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(
        Booking booking,
        List<ShowSeat> showSeats)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Bookings.Add(booking);

            var showSeatIds = showSeats
                .Select(ss => ss.ShowSeatId)
                .ToList();

            await _context.ShowSeats
                .Where(ss => showSeatIds.Contains(ss.ShowSeatId))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(ss => ss.Status, "Booked"));

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return booking.BookingId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await GetBookingQuery()
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task<Booking?> GetByIdForUserAsync(
        int bookingId,
        string userId)
    {
        return await GetBookingQuery()
            .FirstOrDefaultAsync(b =>
                b.BookingId == bookingId &&
                b.UserId == userId);
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await GetBookingQuery()
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(string userId)
    {
        return await GetBookingQuery()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task CancelAsync(
        Booking booking,
        List<ShowSeat> showSeats)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            booking.Status = "Cancelled";

            var showSeatIds = showSeats
                .Select(ss => ss.ShowSeatId)
                .ToList();

            await _context.ShowSeats
                .Where(ss => showSeatIds.Contains(ss.ShowSeatId))
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(ss => ss.Status, "Available"));

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private IQueryable<Booking> GetBookingQuery()
    {
        return _context.Bookings
            .Include(b => b.Show)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Show)
                .ThenInclude(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat);
    }
}