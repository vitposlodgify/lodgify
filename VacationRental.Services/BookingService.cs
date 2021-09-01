using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Data.Models;
using VacationRental.Data.Repositories;

namespace VacationRental.Services
{
    public interface IBookingService
    {
        Task<ResourceIdViewModel> CreateBookingAsync(BookingViewModel item);
        Task<BookingViewModel> GetBookingAsync(int bookingId);
        Task<IEnumerable<BookingViewModel>> GetAllBookingsByRentalAsync(int rentalId);
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalService _rentalService;

        public BookingService(IBookingRepository bookingRepository, IRentalService rentalService)
        {
            _bookingRepository = bookingRepository;
            _rentalService = rentalService;
        }

        public async Task<ResourceIdViewModel> CreateBookingAsync(BookingViewModel item)
        {
            var rental = await _rentalService.GetRentalAsync(item.RentalId);
            var bookings = await GetAllBookingsByRentalAsync(item.RentalId);

            int unit = 0;
            for (var i = 0; i < item.Nights + rental.PreparationTimeInDays; i++)
            {
                unit = bookings
                            .Count(x => (x.Start <= item.Start.Date && x.Start.AddDays(x.Nights) > item.Start.Date)
                                        || (x.Start < item.Start.AddDays(item.Nights) && x.Start.AddDays(x.Nights) >= item.Start.AddDays(item.Nights))
                                        || (x.Start > item.Start && x.Start.AddDays(x.Nights) < item.Start.AddDays(item.Nights)));
            }

            item.Unit = ++unit;
            return await _bookingRepository.CreateBookingAsync(item);
        }

        public async Task<IEnumerable<BookingViewModel>> GetAllBookingsByRentalAsync(int rentalId)
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return bookings.Where(x => x.RentalId == rentalId);
        }

        public Task<BookingViewModel> GetBookingAsync(int bookingId)
        {
            return _bookingRepository.FindBookingAsync(bookingId);
        }
    }
}
