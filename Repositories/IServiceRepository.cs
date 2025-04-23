using BookingService.Models;
using BookingService.Specifications;

namespace BookingService.Repositories
{
    public interface IServiceRepository
    {
        Task<Service> GetByIdAsync(int id, ServiceSpecifications? spec = null);
        Task<List<Service>> GetByIdsAsync(List<int> ids, ServiceSpecifications? spec = null);
        Task<List<Service>> GetAllByServiceGroupId(int serviceGroupId);
        Task AddAsync(Service service);
        Task DeleteAsync(Service service);
        Task UpdateRangeAsync(List<Service> services);
        Task UpdateAsync(Service service);
    }
}
