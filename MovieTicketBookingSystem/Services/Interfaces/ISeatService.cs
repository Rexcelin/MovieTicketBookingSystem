using MovieTicketBookingSystem.DTOs.Seats;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface ISeatService
{
    Task<SeatResponse> CreateSeatAsync(CreateSeatRequest request);

    Task<List<SeatResponse>> CreateSeatsBulkAsync(
        List<CreateSeatRequest> requests);

    Task<SeatResponse?> GetSeatByIdAsync(int seatId);

    Task<List<SeatResponse>> GetAllSeatsAsync();

    Task<List<SeatResponse>> GetSeatsByScreenIdAsync(int screenId);

    Task<SeatResponse?> UpdateSeatAsync(
        int seatId,
        UpdateSeatRequest request);

    Task<bool> DeleteSeatAsync(int seatId);
}