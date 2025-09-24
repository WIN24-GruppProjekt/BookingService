
using Persistence.Contexts;
using Persistence.Entities;

namespace Persistence.Repositories;

public class BookingRepository(DataContext context) : BaseRepository<BookingEntity>(context), IBookingRepository
{
}
