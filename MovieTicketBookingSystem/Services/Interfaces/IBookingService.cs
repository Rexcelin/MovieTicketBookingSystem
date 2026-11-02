
using MovieTicketBookingSystem.DTOs.Bookings;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(
        CreateBookingRequest request);

    Task<BookingResponse?> GetBookingByIdAsync(int bookingId);

    Task<List<BookingResponse>> GetAllBookingsAsync();

    Task<bool> CancelBookingAsync(int bookingId);
}
