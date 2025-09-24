
namespace Application.DTOs;

public class CreateBookingRequest
{
    public string UserId { get; set; } = null!; 

    public string EventId { get; set; } = null!; 

    // No field is needed in the frontend for "CreateAt" this, this is handled by the CreateBookingAsync service.
}
