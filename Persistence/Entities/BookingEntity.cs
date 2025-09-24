

using System.ComponentModel.DataAnnotations;

namespace Persistence.Entities;

public class BookingEntity // The table generated functions as a ledger for bookings
{
    // Datatype for all IDs is string to be easier for testing and to be more flexible in case of Business requirements. 
    // Allows for custom ID formats by changing the services, not running new migrations and changing entities.

    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();  
    [Required]
    public string UserId { get; set; } = null!; // Comes from AccountService, bound to the logged-in user 
    [Required]
    public string EventId { get; set; } = null!; // Comes from EventService, bound to the booking button on the event
    [Required] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // This should be set automatically to time of creation for bookkeeping purposes.

    public int? ActiveParticipants { get; set; } = 0; // This is the number of participants signed up for the event.
                                                      // This should always be equal to the number of times an eventId appears in the table

    //public string? Status { get; set; } = "Active"; // Status needs information from EventService to be set to "Cancelled" or similar. This can be done in the frontend (inefficient) but until we have a monolithic architecture, I don't see how to do this properly here


}
