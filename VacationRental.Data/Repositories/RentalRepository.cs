using System.Collections.Generic;
using System.Threading.Tasks;
using VacationRental.Data.Models;

namespace VacationRental.Data.Repositories
{
    public interface IRentalRepository
    {
        Task<IEnumerable<RentalViewModel>> GetAllRentalsAsync();
        Task<RentalViewModel> FindRentalAsync(int rentalId);
        Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel item);
        Task UpdateRentalAsync(RentalViewModel item);
    }

    public class RentalRepository : IRentalRepository
    {
        private readonly VacationContext _context;

        public RentalRepository(VacationContext context)
        {
            _context = context;
        }

        public async Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel item)
        {
            var key = new ResourceIdViewModel { Id = _context.Rentals.Keys.Count + 1 };
            item.Id = key.Id;
            _context.Rentals.Add(key.Id, item);

            return key;
        }

        public async Task<RentalViewModel> FindRentalAsync(int rentalId)
        {
            if (!_context.Rentals.ContainsKey(rentalId))
            {
                return default;
            }

            return _context.Rentals[rentalId];
        }

        public async Task<IEnumerable<RentalViewModel>> GetAllRentalsAsync()
        {
            return _context.Rentals.Values;
        }

        public async Task UpdateRentalAsync(RentalViewModel item)
        {
            _context.Rentals[item.Id] = item;
        }
    }
}
