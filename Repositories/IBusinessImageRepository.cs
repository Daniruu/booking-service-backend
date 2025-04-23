using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IBusinessImageRepository
    {
        Task<List<BusinessImage>> GetByBusinessIdAsync(int businessId);
        Task<BusinessImage?> GetByIdAsync(int id);
        Task AddAsync(BusinessImage image);
        Task UpdateAsync(BusinessImage image);
        Task DeleteAsync(BusinessImage image);
        Task DeleteAllByBusinessIdAsync(int businessId);
    }
}
