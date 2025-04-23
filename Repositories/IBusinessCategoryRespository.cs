using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IBusinessCategoryRespository
    {
        Task AddAsync(BusinessCategory businessCategory);
        Task UpdateCategory(BusinessCategory businessCategory);
        Task RemoveCategory(BusinessCategory businessCategory);
        Task<BusinessCategory?> GetByIdAsync(int id);
        Task<BusinessCategory?> GetByNameAsync(string name);
    }
}
