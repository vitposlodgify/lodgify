using FluentValidation;
using System.Linq;
using VacationRental.Data.Models;
using VacationRental.Services;

namespace VacationRental.Api.Validations
{
    public class RentalViewModelValidator : AbstractValidator<RentalViewModel>
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;

        public RentalViewModelValidator(IRentalService rentalService, IBookingService bookingService)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id doesn't exists");

            RuleFor(x => x.Id)
                .Must(CheckIfRentalExists)
                .WithMessage("Rental does not exists");

            RuleFor(x => x.Units)
                .GreaterThan(0)
                .WithMessage("Nights must be positive");

            RuleFor(x => x.PreparationTimeInDays)
                .GreaterThan(0)
                .WithMessage("Preparation time must be positive");

            RuleFor(x => x)
                .Must(CheckIfPreparationTimeOverlap)
                .WithMessage("Preparation time could not be change according to existing booking");
        }

        private bool CheckIfRentalExists(int rentalId)
        {
            var rental = _rentalService.GetRentalAsync(rentalId).Result;
            return rental != default;
        }

        private bool CheckIfPreparationTimeOverlap(RentalViewModel model)
        {
            var bookings = _bookingService.GetAllBookingsByRentalAsync(model.Id).Result.ToArray();

            if(bookings.Length < 2)
            {
                return true;
            }

            for(int i = 0; i < bookings.Length - 1; i++)
            {
                if (bookings[i].Start.AddDays(bookings[i].Nights + model.PreparationTimeInDays) >= bookings[i + 1].Start)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
