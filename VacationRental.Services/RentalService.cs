using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Data.Models;
using VacationRental.Data.Repositories;

namespace VacationRental.Services
{
    public interface IRentalService
    {
        Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel item);
        Task<RentalViewModel> GetRentalAsync(int rentalId);
        Task<IEnumerable<RentalViewModel>> GetAllRentalsAsync();
        Task UpdateRentalAsync(RentalViewModel item);
    }

    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;

        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        public async Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel item)
        {
            return await _rentalRepository.CreateRentalAsync(item);
        }

        public async Task<IEnumerable<RentalViewModel>> GetAllRentalsAsync()
        {
            return await _rentalRepository.GetAllRentalsAsync();
        }

        public async Task<RentalViewModel> GetRentalAsync(int rentalId)
        {
            return await _rentalRepository.FindRentalAsync(rentalId);
        }

        public async Task UpdateRentalAsync(RentalViewModel item)
        {
            await _rentalRepository.UpdateRentalAsync(item);
        }
    }
}
