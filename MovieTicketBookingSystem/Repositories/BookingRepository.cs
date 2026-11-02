
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
            // Add booking and its BookingSeats
            _context.Bookings.Add(booking);

            // Update only the ShowSeat status in the database
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
        return await _context.Bookings
            .Include(b => b.Show)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Show)
                .ThenInclude(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.Show)
                .ThenInclude(s => s.Movie)
            .Include(b => b.Show)
                .ThenInclude(s => s.Screen)
                    .ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
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
}
