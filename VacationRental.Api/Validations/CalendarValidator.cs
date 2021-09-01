using FluentValidation;
using VacationRental.Api.Model;
using VacationRental.Services;

namespace VacationRental.Api.Validations
{
    public class CalendarValidator : AbstractValidator<CalendarValidation>
    {
        private readonly IRentalService _rentalService;

        public CalendarValidator(IRentalService rentalService)
        {
            _rentalService = rentalService;

            RuleFor(x => x.Nights)
                .GreaterThan(0)
                .WithMessage("Nights must be positive");

            RuleFor(x => x.RentalId)
                .Must(CheckIfRentalExists)
                .WithMessage("Rental not found");
        }

        private bool CheckIfRentalExists(int rentalId)
        {
            var rental = _rentalService.GetRentalAsync(rentalId).Result;
            return rental != default;
        }
    }
}
