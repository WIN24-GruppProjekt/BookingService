
using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class Booking
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!; 

    public string EventId { get; set; } = null!; 

    public DateTime? CreatedAt { get; set; } // Setting this as a nullable for the model can be useful to catch if the mapping from Entity to Model fails to set time correctly.  

    //public string? Status { get; set; } = "Active"; // No support for this, see BookingEntity.cs

    public int? ActiveParticipants { get; set; } = 0; // This is updated in the READ method in BookingService.cs
}
