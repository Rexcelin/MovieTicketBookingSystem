using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Bookings;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AdminOrCustomer")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    // POST: api/bookings
    // Admin and Customer can create bookings for their own accounts.
    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var booking = await _bookingService.CreateBookingAsync(
                request,
                userId);

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = booking.BookingId },
                booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/bookings
    // Admin: all bookings
    // Customer: own bookings only
    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var userId = IsAdmin() ? null : GetCurrentUserId();

        if (string.IsNullOrWhiteSpace(userId) && !IsAdmin())
            return Unauthorized();

        var bookings = await _bookingService.GetAllBookingsAsync(userId);

        return Ok(bookings);
    }

    // GET: api/bookings/1
    // Admin: any booking
    // Customer: own booking only
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        try
        {
            var userId = IsAdmin() ? null : GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId) && !IsAdmin())
                return Unauthorized();

            var booking = await _bookingService.GetBookingByIdAsync(
                id,
                userId);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return Ok(booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/bookings/1/cancel
    // Admin: cancel any booking
    // Customer: cancel own booking only
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            var userId = IsAdmin() ? null : GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(userId) && !IsAdmin())
                return Unauthorized();

            bool cancelled = await _bookingService.CancelBookingAsync(
                id,
                userId);

            if (!cancelled)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return Ok(new
            {
                message = "Booking cancelled successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}