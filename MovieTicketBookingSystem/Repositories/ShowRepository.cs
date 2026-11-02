
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Show> AddAsync(Show show)
    {
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        return show;
    }

    public async Task<Show?> GetByIdAsync(int showId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .FirstOrDefaultAsync(s => s.ShowId == showId);
    }

    public async Task<List<Show>> GetAllAsync()
    {
        return await _context.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> GetByMovieIdAsync(int movieId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .Where(s => s.MovieId == movieId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> GetByScreenIdAsync(int screenId)
    {
        return await _context.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Screen)
            .Where(s => s.ScreenId == screenId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasScheduleOverlapAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null)
    {
        var query = _context.Shows
            .Where(s =>
                s.ScreenId == screenId &&
                startTime < s.EndTime &&
                endTime > s.StartTime);

        if (excludeShowId.HasValue)
        {
            query = query.Where(
                s => s.ShowId != excludeShowId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task UpdateAsync(Show show)
    {
        _context.Shows.Update(show);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int showId)
    {
        var show = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == showId);

        if (show == null)
            return false;

        _context.Shows.Remove(show);
        await _context.SaveChangesAsync();

        return true;
    }
}
