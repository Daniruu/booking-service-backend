using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class ServiceGroupRepository : IServiceGroupRepository
    {
        private readonly BookingServiceDbContext _context;

        public ServiceGroupRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ServiceGroup serviceGroup)
        {
            await _context.ServiceGroups.AddAsync(serviceGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceGroup> GetByIdAsync(int id)
        {
            var serviceGroup = await _context.ServiceGroups.Where(sg => sg.Id == id).Include(sg => sg.Services).FirstOrDefaultAsync();

            if (serviceGroup != null)
                serviceGroup.Services = serviceGroup.Services.OrderBy(s => s.Order).ToList();

            return serviceGroup;
        }
        public async Task DeleteAsync(ServiceGroup serviceGroup)
        {
            _context.ServiceGroups.Remove(serviceGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceGroup>> GetAllByBusinessIdAsync(int businessId)
        {
            return await _context.ServiceGroups
                .Where(sg => sg.BusinessId == businessId)
                .Include(sg => sg.Services)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(List<ServiceGroup> serviceGroups)
        {
            _context.ServiceGroups.UpdateRange(serviceGroups);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceGroup serviceGroup)
        {
            _context.ServiceGroups.Update(serviceGroup);
            await _context.SaveChangesAsync();
        }
    }
}
