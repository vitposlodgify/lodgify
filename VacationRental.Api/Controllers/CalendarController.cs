using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VacationRental.Api.Model;
using VacationRental.Services;
using VacationRental.Services.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;
        private readonly IValidator<CalendarValidation> _validator;

        public CalendarController(
            ICalendarService calendarService,
            IValidator<CalendarValidation> validator)
        {
            _calendarService = calendarService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<CalendarViewModel>> GetBookingCalendar(int rentalId, DateTime start, int nights)
        {
            var validationResult = _validator.Validate(new CalendarValidation() { Nights = nights, RentalId = rentalId});
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            return await _calendarService.GetCalendarAsync(rentalId, start, nights);
        }
    }
}
