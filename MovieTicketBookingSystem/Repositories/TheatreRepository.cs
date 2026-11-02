using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class TheatreRepository : ITheatreRepository
{
    private readonly ApplicationDbContext _context;

    public TheatreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Theatre> AddAsync(Theatre theatre)
    {
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();

        return theatre;
    }

    public async Task<Theatre?> GetByIdAsync(int theatreId)
    {
        return await _context.Theatres
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TheatreId == theatreId);
    }

    public async Task<List<Theatre>> GetAllAsync()
    {
        return await _context.Theatres
            .AsNoTracking()
            .OrderBy(t => t.TheatreId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Theatre theatre)
    {
        _context.Theatres.Update(theatre);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int theatreId)
    {
        var theatre = await _context.Theatres
            .FirstOrDefaultAsync(t => t.TheatreId == theatreId);

        if (theatre == null)
        {
            return false;
        }

        _context.Theatres.Remove(theatre);
        await _context.SaveChangesAsync();

        return true;
    }
}