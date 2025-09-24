using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _bookingService.CreateBookingAsync(request);
        return result.Success
            ? Ok(result)
            : BadRequest(result.Error);
    }

    // GET: api/bookings/user/{userId}
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBookingsByUserId(string userId)
    {
        var result = await _bookingService.GetBookingsByUserIdAsync(userId);
        if (result.Success)
        {
            return Ok(result.Result);
        }
        return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }

    // GET: api/bookings/event/{eventId}/participants
    [HttpGet("event/{eventId}/participants")]
    public async Task<IActionResult> GetActiveParticipantsForEvent(string eventId)
    {
        var result = await _bookingService.GetActiveParticipantsForEventAsync(eventId);
        if (result.Success)
        {
            return Ok(result.Result);
        }
        return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }

    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> DeleteBooking(string bookingId)
    {
        var result = await _bookingService.DeleteBookingAsync(bookingId);
        return result.Success
            ? Ok(result)
            : NotFound(result.Error);
    }

    // DELETE: api/bookings/user/{userId}
    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> DeleteBookingsForUser(string userId)
    {
        var result = await _bookingService.DeleteBookingsForUser(userId);
        return result.Success
            ? Ok(result)
            : NotFound(result.Error);
    }

    // DELETE: api/bookings/event/{eventId}
    [HttpDelete("event/{eventId}")]
    public async Task<IActionResult> DeleteBookingsForEvent(string eventId)
    {
        var result = await _bookingService.DeleteBookingsForEvent(eventId);
        return result.Success
            ? Ok(result)
            : NotFound(result.Error);
    }
}
