using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly ApplicationDbContext _context;

    public ScreenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Screen> AddAsync(Screen screen)
    {
        await _context.Screens.AddAsync(screen);
        await _context.SaveChangesAsync();

        return screen;
    }

    public async Task<Screen?> GetByIdAsync(int screenId)
    {
        return await _context.Screens
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ScreenId == screenId);
    }

    public async Task<List<Screen>> GetAllAsync()
    {
        return await _context.Screens
            .AsNoTracking()
            .OrderBy(s => s.ScreenId)
            .ToListAsync();
    }

    public async Task<List<Screen>> GetByTheatreIdAsync(int theatreId)
    {
        return await _context.Screens
            .AsNoTracking()
            .Where(s => s.TheatreId == theatreId)
            .OrderBy(s => s.ScreenId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Screen screen)
    {
        _context.Screens.Update(screen);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int screenId)
    {
        var screen = await _context.Screens
            .FirstOrDefaultAsync(s => s.ScreenId == screenId);

        if (screen == null)
        {
            return false;
        }

        _context.Screens.Remove(screen);
        await _context.SaveChangesAsync();

        return true;
    }
}