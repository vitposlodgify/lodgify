using System.Collections.Generic;
using VacationRental.Data.Models;

namespace VacationRental.Data
{
    public class VacationContext
    {
        private readonly IDictionary<int, RentalViewModel> _rentals;
        private readonly IDictionary<int, BookingViewModel> _bookings;

        public IDictionary<int, RentalViewModel> Rentals { get => _rentals; }
        public IDictionary<int, BookingViewModel> Bookings { get => _bookings; }

        public VacationContext()
        {
            _rentals = new Dictionary<int, RentalViewModel>();
            _bookings = new Dictionary<int, BookingViewModel>();
        }
    }
}
