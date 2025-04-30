using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BusinessCategoryRepository : IBusinessCategoryRespository
    {
        private readonly BookingServiceDbContext _context;

        public BusinessCategoryRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessCategory>> GetAll()
        {
            return await _context.BusinessCategories.ToListAsync();
        }
        public async Task AddAsync(BusinessCategory businessCategory)
        {
            await _context.BusinessCategories.AddAsync(businessCategory);
        }

        public async Task UpdateCategory(BusinessCategory businessCategory)
        {
            _context.BusinessCategories.Update(businessCategory);
        }

        public async Task RemoveCategory(BusinessCategory businessCategory)
        {
            _context.BusinessCategories.Remove(businessCategory);
        }

        public async Task<BusinessCategory?> GetByIdAsync(int id)
        {
            return await _context.BusinessCategories.FindAsync(id);
        }

        public async Task<BusinessCategory?> GetByNameAsync(string name)
        {
            return await _context.BusinessCategories.FirstOrDefaultAsync(bc => bc.Name == name);
        }
    }
}
