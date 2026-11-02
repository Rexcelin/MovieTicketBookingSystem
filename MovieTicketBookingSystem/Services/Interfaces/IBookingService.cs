using MovieTicketBookingSystem.DTOs.Bookings;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(
        CreateBookingRequest request,
        string userId);

    Task<BookingResponse?> GetBookingByIdAsync(
        int bookingId,
        string? userId = null);

    Task<List<BookingResponse>> GetAllBookingsAsync(
        string? userId = null);

    Task<bool> CancelBookingAsync(
        int bookingId,
        string? userId = null);
}