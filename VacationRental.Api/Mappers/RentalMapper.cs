using AutoMapper;
using VacationRental.Data.Models;
using VacationRental.Services.Models;

namespace VacationRental.Api.Mappers
{
    public class RentalMapper : Profile
    {
        public RentalMapper()
        {
            CreateMap<RentalBindingModel, RentalViewModel>();
        }
    }
}
