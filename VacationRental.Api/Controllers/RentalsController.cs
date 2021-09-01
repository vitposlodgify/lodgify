using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Data.Models;
using VacationRental.Services;
using VacationRental.Services.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IMapper _mapper;
        private readonly IValidator<RentalBindingModel> _rentalBindingModelValidator;
        private readonly IValidator<RentalViewModel> _rentalViewModelValidator;

        public RentalsController(
            IRentalService rentalService,
            IMapper mapper,
            IValidator<RentalBindingModel> rentalBindingModelValidator,
            IValidator<RentalViewModel> rentalViewModelValidator)
        {
            _rentalService = rentalService;
            _mapper = mapper;
            _rentalBindingModelValidator = rentalBindingModelValidator;
            _rentalViewModelValidator = rentalViewModelValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalViewModel>>> GetRentalsAsync()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            return Ok(rentals);
        }

        [HttpGet("{rentalId:int}")]
        public async Task<ActionResult<RentalViewModel>> GetRentalAsync(int rentalId)
        {
            var rental = await _rentalService.GetRentalAsync(rentalId);

            if(rental == default)
            {
                return BadRequest("Rental not found");
            }

            return Ok(rental);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceIdViewModel>> CreateRentalAsync(RentalBindingModel model)
        {
            var validationResult = _rentalBindingModelValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var rentalId = await _rentalService.CreateRentalAsync(_mapper.Map<RentalViewModel>(model));
            return new CreatedResult(nameof(GetRentalAsync), rentalId);
        }

        [HttpPut]
        public async Task<ActionResult<ResourceIdViewModel>> UpdateRentalAsync(RentalViewModel model)
        {
            var validationResult = _rentalViewModelValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            await _rentalService.UpdateRentalAsync(model);
            return Ok();
        }
    }
}
