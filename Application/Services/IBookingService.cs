using Application.DTOs;
using Application.Models;

namespace Application.Services
{
    public interface IBookingService
    {
        Task<BookingResult> CreateBookingAsync(CreateBookingRequest request);
        Task<BookingResult> DeleteBookingAsync(string bookingId);
        Task<BookingResult> DeleteBookingsForEvent(string eventId);
        Task<BookingResult> DeleteBookingsForUser(string userId);
        Task<BookingResult<int>> GetActiveParticipantsForEventAsync(string eventId);
        Task<BookingResult<List<Booking>>> GetBookingsByUserIdAsync(string userId);
    }
}