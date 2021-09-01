using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VacationRental.Data.Models;
using VacationRental.Services;
using VacationRental.Services.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IValidator<BookingBindingModel> _validator;

        public BookingsController(
            IRentalService rentalService,
            IBookingService bookingService,
            IMapper mapper,
            IValidator<BookingBindingModel> validator)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpGet("{bookingId:int}")]
        public async Task<ActionResult<BookingViewModel>> GetBookingAsync(int bookingId)
        {
            var booking = await _bookingService.GetBookingAsync(bookingId);

            if (booking == default)
            {
                return BadRequest("Booking not found");
            }

            return booking;
        }

        [HttpPost]
        public async Task<ActionResult<ResourceIdViewModel>> CreateBookingAsync([FromBody]BookingBindingModel model)
        {
            var validationResult = _validator.Validate(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var bookingId = await _bookingService.CreateBookingAsync(_mapper.Map<BookingViewModel>(model));
            return new CreatedResult(nameof(GetBookingAsync), bookingId);
        }
    }
}
