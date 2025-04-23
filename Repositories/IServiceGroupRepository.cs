using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IServiceGroupRepository
    {
        Task AddAsync(ServiceGroup serviceGroup);
        Task<ServiceGroup> GetByIdAsync(int id);
        Task DeleteAsync(ServiceGroup serviceGroup);
        Task<List<ServiceGroup>> GetAllByBusinessIdAsync(int businessId);
        Task UpdateRangeAsync(List<ServiceGroup> serviceGroups);
        Task UpdateAsync(ServiceGroup serviceGroup);
    }
}
