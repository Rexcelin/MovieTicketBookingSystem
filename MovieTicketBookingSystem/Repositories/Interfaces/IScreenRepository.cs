using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IScreenRepository
{
    Task<Screen> AddAsync(Screen screen);

    Task<Screen?> GetByIdAsync(int screenId);

    Task<List<Screen>> GetAllAsync();

    Task<List<Screen>> GetByTheatreIdAsync(int theatreId);

    Task UpdateAsync(Screen screen);

    Task<bool> DeleteAsync(int screenId);
}