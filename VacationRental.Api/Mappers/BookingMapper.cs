using AutoMapper;
using VacationRental.Data.Models;
using VacationRental.Services.Models;

namespace VacationRental.Api.Mappers
{
    public class BookingMapper : Profile
    {
        public BookingMapper()
        {
            CreateMap<BookingBindingModel, BookingViewModel>();
        }
    }
}
