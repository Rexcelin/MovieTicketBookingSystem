using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.DTOs.Theatres;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TheatresController : ControllerBase
{
    private readonly ITheatreService _service;

    public TheatresController(ITheatreService service)
    {
        _service = service;
    }

    // POST: api/theatres
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<ActionResult<TheatreResponse>> CreateTheatre(
        [FromBody] CreateTheatreRequest request)
    {
        try
        {
            var theatre = await _service.CreateTheatreAsync(request);

            return CreatedAtAction(
                nameof(GetTheatreById),
                new { id = theatre.TheatreId },
                theatre);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/theatres
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet]
    public async Task<ActionResult<List<TheatreResponse>>> GetAllTheatres()
    {
        var theatres = await _service.GetAllTheatresAsync();

        return Ok(theatres);
    }

    // GET: api/theatres/1
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TheatreResponse>> GetTheatreById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid theatre ID." });
        }

        var theatre = await _service.GetTheatreByIdAsync(id);

        if (theatre == null)
        {
            return NotFound(new { message = "Theatre not found." });
        }

        return Ok(theatre);
    }

    // PUT: api/theatres/1
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TheatreResponse>> UpdateTheatre(
        int id,
        [FromBody] UpdateTheatreRequest request)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid theatre ID." });
        }

        var theatre = await _service.UpdateTheatreAsync(id, request);

        if (theatre == null)
        {
            return NotFound(new { message = "Theatre not found." });
        }

        return Ok(theatre);
    }

    // DELETE: api/theatres/1
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTheatre(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid theatre ID." });
        }

        try
        {
            var deleted = await _service.DeleteTheatreAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Theatre not found." });
            }

            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "This theatre cannot be deleted because it has associated screens."
            });
        }
    }
}