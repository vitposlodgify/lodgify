using FluentValidation;
using System.Linq;
using VacationRental.Services;
using VacationRental.Services.Models;

namespace VacationRental.Api.Validations
{
    public class BookingValidator : AbstractValidator<BookingBindingModel>
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;

        public BookingValidator(IRentalService rentalService, IBookingService bookingService)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;

            RuleFor(x => x.Nights)
                .GreaterThan(0)
                .WithMessage("Nigts must be positive");

            RuleFor(x => x)
                .Must(CheckIfRentalExists)
                .WithMessage("Rental not found");

            RuleFor(x => x)
                .Must(CheckIfRentalAvailable)
                .WithMessage("Not available");
        }

        private bool CheckIfRentalExists(BookingBindingModel model)
        {
            var rental = _rentalService.GetRentalAsync(model.RentalId).Result;
            return rental != default;
        }

        private bool CheckIfRentalAvailable(BookingBindingModel model)
        {
            var rental = _rentalService.GetRentalAsync(model.RentalId).Result;
            if(rental == default)
            {
                return true;
            }

            var bookings = _bookingService.GetAllBookingsByRentalAsync(rental.Id).Result;

            for (var i = 0; i < model.Nights + rental.PreparationTimeInDays; i++)
            {
                var count = bookings
                            .Count(x => (x.Start <= model.Start.Date && x.Start.AddDays(x.Nights) > model.Start.Date)
                                        || (x.Start < model.Start.AddDays(model.Nights) && x.Start.AddDays(x.Nights) >= model.Start.AddDays(model.Nights))
                                        || (x.Start > model.Start && x.Start.AddDays(x.Nights) < model.Start.AddDays(model.Nights)));

                if (count >= rental.Units)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
