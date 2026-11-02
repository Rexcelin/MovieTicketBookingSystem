using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface ITheatreRepository
{
    Task<Theatre> AddAsync(Theatre theatre);

    Task<Theatre?> GetByIdAsync(int theatreId);

    Task<List<Theatre>> GetAllAsync();

    Task UpdateAsync(Theatre theatre);

    Task<bool> DeleteAsync(int theatreId);
}