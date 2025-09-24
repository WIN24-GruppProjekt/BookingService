
using Application.DTOs;
using Application.Models;
using Persistence.Entities;
using Persistence.Repositories;

namespace Application.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    // If synchronous, bookings should be fetched first via GetBookingsByUserIdAsync, then EventService to get Events by Id. Update is not needed since none of the fields shouuld be changed by the user or the system.
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task<BookingResult> CreateBookingAsync(CreateBookingRequest request)
    {
        try
        {
            // Check if the user has already booked this event
            var alreadyExists = await _bookingRepository.AlreadyExistsAsync(
                b => b.UserId == request.UserId && b.EventId == request.EventId
            );

            if (alreadyExists.Success)
            {
                return new BookingResult
                {
                    Success = false,
                    Error = "User has already booked this event."
                };
            }

            // Proceeds to create booking if no existing booking is found
            var bookingEntity = new BookingEntity
            {
                UserId = request.UserId,
                EventId = request.EventId,
                CreatedAt = DateTime.UtcNow, // Ensure the creation time is set to the time the service is called, not the time the request is created.
                ActiveParticipants = null // This can be null here and then assigned/updated whenever bookings are fetched.
            };
            var result = await _bookingRepository.AddAsync(bookingEntity);
            return result.Success
                ? new BookingResult { Success = true }
                : new BookingResult { Success = false, Error = result.Error };
        }
        catch (Exception ex)
        {
            return new BookingResult { Success = false, Error = ex.Message };
        }
    }

    // This is not necessarily more lightweight than fetching all bookings since this can quickly spiral to N+1 requests. However, there are instances where the only information needed from the DB is active participants (i.e. to see if "Book Now" button should be active or not in frontend)
    public async Task<BookingResult<int>> GetActiveParticipantsForEventAsync(string eventId)
    {
        try
        {
            var countResult = await _bookingRepository.CountAsync(b => b.EventId == eventId);

            if (!countResult.Success)
            {
                return new BookingResult<int>
                {
                    Success = false,
                    Error = countResult.Error
                };
            }

            return new BookingResult<int>
            {
                Success = true,
                Result = countResult.Result
            };
        }
        catch (Exception ex)
        {
            return new BookingResult<int>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }


    // Fetches all bookings for a user. The ActiveParticipants field is updated here to reflect the number of bookings for the event.
    // Note: ActiveParticipants is done terribly here. In monolithic architecture, this is resolved much cleaner and with much higher efficiency since the count is updated upon booking dierctly on the Event entity. Here we have to query on reload to update which is jank as hell. This can be improved by doing this using SQL commands instead of doing everything in memory, but I'm not sure that effort is worth it.
    public async Task<BookingResult<List<Booking>>> GetBookingsByUserIdAsync(string userId)
    {
        try
        {
            var bookingsResult = await _bookingRepository.GetAllAsync(b => b.UserId == userId);

            if (!bookingsResult.Success)
            {
                return new BookingResult<List<Booking>>
                {
                    Success = false,
                    Error = bookingsResult.Error
                };
            }

            var userBookings = bookingsResult.Result!;

            // Get all distinct EventIds for the user's bookings
            var eventIds = userBookings.Select(b => b.EventId).Distinct().ToList(); // Distinct to avoid redundant counts

            // List of all events shared by target user and all other users
            var allBookingsForEvents = await _bookingRepository.GetAllAsync(b => eventIds.Contains(b.EventId));

            var eventCounts = allBookingsForEvents.Result!
                .GroupBy(b => b.EventId)  // Group by EventId
                .ToDictionary(g => g.Key, g => g.Count()); // Create a dictionary with EventId as key and its count as value

            // Map to Booking model
            var bookings = userBookings
                .Select(b => new Booking
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    EventId = b.EventId,
                    CreatedAt = b.CreatedAt,
                    ActiveParticipants = eventCounts.ContainsKey(b.EventId) ? eventCounts[b.EventId] : 0
                })
                .ToList();

            return new BookingResult<List<Booking>>
            {
                Success = true,
                Result = bookings
            };
        }
        catch (Exception ex)
        {
            return new BookingResult<List<Booking>>
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    // Should be called subsequently in frontend when user is deleted (deleted account for instance)
    public async Task<BookingResult> DeleteBookingsForUser(string userId)
    {
        try
        {
            var bookingsResult = await _bookingRepository.GetAllAsync(b => b.UserId == userId);

            if (!bookingsResult.Success)
            {
                return new BookingResult { Success = false, Error = bookingsResult.Error ?? "Failed to fetch bookings" };
            }

            var bookings = bookingsResult.Result;
            if (bookings == null || !bookings.Any())
            {
                return new BookingResult { Success = false, Error = "No bookings found for user" };
            }

            var deleteResult = await _bookingRepository.DeleteRangeAsync(bookings);
            return deleteResult.Success
                ? new BookingResult { Success = true }
                : new BookingResult { Success = false, Error = deleteResult.Error };
        }
        catch (Exception ex)
        {
            return new BookingResult { Success = false, Error = ex.Message };
        }
    }

    //Should be called when an instructor deletes an event
    public async Task<BookingResult> DeleteBookingsForEvent(string eventId)
    {
        try
        {
            var bookingsResult = await _bookingRepository.GetAllAsync(b => b.EventId == eventId);

            if (!bookingsResult.Success)
            {
                return new BookingResult { Success = false, Error = bookingsResult.Error ?? "Failed to fetch bookings" };
            }

            var bookings = bookingsResult.Result;
            if (bookings == null || !bookings.Any())
            {
                return new BookingResult { Success = false, Error = "No bookings found for event" };
            }

            var deleteResult = await _bookingRepository.DeleteRangeAsync(bookings);
            return deleteResult.Success
                ? new BookingResult { Success = true }
                : new BookingResult { Success = false, Error = deleteResult.Error };
        }
        catch (Exception ex)
        {
            return new BookingResult { Success = false, Error = ex.Message };
        }
    }

    // should be called when a user cancels a booking
    public async Task<BookingResult> DeleteBookingAsync(string bookingId)
    {
        try
        {
            var existingBookingResult = await _bookingRepository.GetAsync(x => x.Id == bookingId);

            if (!existingBookingResult.Success || existingBookingResult.Result == null)
            {
                return new BookingResult { Success = false, Error = "Booking not found" };
            }

            var existingBooking = existingBookingResult.Result;
            var deleteResult = await _bookingRepository.DeleteAsync(existingBooking);
            return deleteResult.Success
                ? new BookingResult { Success = true }
                : new BookingResult { Success = false, Error = deleteResult.Error };
        }
        catch (Exception ex)
        {
            return new BookingResult { Success = false, Error = ex.Message };
        }
    }




}
