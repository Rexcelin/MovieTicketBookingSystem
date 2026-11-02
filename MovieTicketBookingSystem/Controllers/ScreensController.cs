using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.DTOs.Screens;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _service;

    public ScreensController(IScreenService service)
    {
        _service = service;
    }

    // POST: api/screens
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<ScreenResponse>> CreateScreen(
        [FromBody] CreateScreenRequest request)
    {
        try
        {
            var screen = await _service.CreateScreenAsync(request);

            return CreatedAtAction(
                nameof(GetScreenById),
                new { id = screen.ScreenId },
                screen);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/screens
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet]
    public async Task<ActionResult<List<ScreenResponse>>> GetAllScreens()
    {
        var screens = await _service.GetAllScreensAsync();

        return Ok(screens);
    }

    // GET: api/screens/1
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScreenResponse>> GetScreenById(int id)
    {
        try
        {
            var screen = await _service.GetScreenByIdAsync(id);

            if (screen == null)
            {
                return NotFound(new { message = "Screen not found." });
            }

            return Ok(screen);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/screens/theatre/1
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("theatre/{theatreId:int}")]
    public async Task<ActionResult<List<ScreenResponse>>> GetScreensByTheatreId(
        int theatreId)
    {
        try
        {
            var screens = await _service.GetScreensByTheatreIdAsync(theatreId);

            return Ok(screens);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/screens/1
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ScreenResponse>> UpdateScreen(
        int id,
        [FromBody] UpdateScreenRequest request)
    {
        try
        {
            var screen = await _service.UpdateScreenAsync(id, request);

            if (screen == null)
            {
                return NotFound(new { message = "Screen not found." });
            }

            return Ok(screen);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "The screen could not be updated because of a database constraint."
            });
        }
    }

    // DELETE: api/screens/1
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteScreen(int id)
    {
        try
        {
            var deleted = await _service.DeleteScreenAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Screen not found." });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "This screen cannot be deleted because it has associated seats or shows."
            });
        }
    }
}