using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationRental.Services.Models;

namespace VacationRental.Services
{
    public interface ICalendarService
    {
        Task<CalendarViewModel> GetCalendarAsync(int rentalId, DateTime start, int nights);
    }

    public class CalendarService : ICalendarService
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;

        public CalendarService(
            IRentalService rentalService,
            IBookingService bookingService)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;
        }

        public async Task<CalendarViewModel> GetCalendarAsync(int rentalId, DateTime start, int nights)
        {
            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = await GetCalendarDatesAsync(rentalId, start, nights)
            };

            return result;
        }

        private async Task<List<CalendarDateViewModel>> GetCalendarDatesAsync(int rentalId, DateTime start, int nights)
        {
            var calendarDates = new List<CalendarDateViewModel>();
            var rental = await _rentalService.GetRentalAsync(rentalId);

            for (var i = 0; i < nights; i++)
            {
                var bookings = await _bookingService.GetAllBookingsByRentalAsync(rentalId);

                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = bookings
                                .Where(x => x.Start <= start.Date.AddDays(i) && x.Start.AddDays(x.Nights) > start.Date.AddDays(i))
                                .Select(x => new CalendarBookingViewModel { Id = x.Id, Unit = x.Unit })
                                .ToList(),
                    PreparationTimes = bookings
                                .Where(x => x.Start.AddDays(x.Nights) <= start.Date.AddDays(i) && x.Start.AddDays(x.Nights + rental.PreparationTimeInDays) > start.Date.AddDays(i))
                                .Select(x => new PreparationTimeViewModel { Unit = x.Unit })
                                .ToList()
                };

                calendarDates.Add(date);
            }

            return calendarDates;
        }
    }
}
