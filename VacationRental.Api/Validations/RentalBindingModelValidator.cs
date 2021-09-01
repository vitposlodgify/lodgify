using FluentValidation;
using VacationRental.Services.Models;

namespace VacationRental.Api.Validations
{
    public class RentalBindingModelValidator : AbstractValidator<RentalBindingModel>
    {
        public RentalBindingModelValidator()
        {
            RuleFor(x => x.Units)
                .GreaterThan(0)
                .WithMessage("Nights must be positive");

            RuleFor(x => x.PreparationTimeInDays)
                .GreaterThan(0)
                .WithMessage("Preparation time must be positive");
        }
    }
}
