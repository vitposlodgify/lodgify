using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Data.Models;

namespace VacationRental.Data.Repositories
{
    public interface IBookingRepository
    {
        Task<ResourceIdViewModel> CreateBookingAsync(BookingViewModel item);
        Task<BookingViewModel> FindBookingAsync(int bookingId);
        Task<IEnumerable<BookingViewModel>> GetAllBookingsAsync();
    }

    public class BookingRepository : IBookingRepository
    {
        private readonly VacationContext _context;

        public BookingRepository(VacationContext context)
        {
            _context = context;
        }

        public async Task<ResourceIdViewModel> CreateBookingAsync(BookingViewModel item)
        {
            var key = new ResourceIdViewModel { Id = _context.Bookings.Keys.Count + 1 };
            item.Id = key.Id;
            _context.Bookings.Add(key.Id, item);

            return key;
        }

        public async Task<BookingViewModel> FindBookingAsync(int bookingId)
        {
            if (!_context.Bookings.ContainsKey(bookingId))
            {
                return default;
            }

            return _context.Bookings[bookingId];
        }

        public async Task<IEnumerable<BookingViewModel>> GetAllBookingsAsync()
        {
            return _context.Bookings.Values;
        }
    }
}
