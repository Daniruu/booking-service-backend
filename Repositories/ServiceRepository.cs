using BookingService.Data;
using BookingService.Models;
using BookingService.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly BookingServiceDbContext _context;

        public ServiceRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Service> GetByIdAsync(int id, ServiceSpecifications? spec = null)
        {
            var query = _context.Services.AsQueryable();

            if (spec?.IncludeBusiness == true)
                query = query.Include(s => s.Business)
                    .ThenInclude(b => b.Schedule)
                    .ThenInclude(s => s.TimeSlots)
                    .Include(s => s.Business.Settings);

            if (spec?.IncludeEmployee == true)
                query = query.Include(s => s.Employee)
                    .ThenInclude(e => e.Schedule)
                    .ThenInclude(s => s.TimeSlots);

            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }
        
        public async Task<List<Service>> GetByIdsAsync(List<int> ids, ServiceSpecifications? spec = null)
        {
            if (ids == null || !ids.Any())
                return new List<Service>();

            var query = _context.Services.AsQueryable();

            if (spec != null)
            {
                if (spec?.IncludeBusiness == true)
                    query = query.Include(s => s.Business)
                        .ThenInclude(b => b.Schedule)
                        .ThenInclude(s => s.TimeSlots)
                        .Include(s => s.Business.Settings);

                if (spec?.IncludeEmployee == true)
                    query = query.Include(s => s.Employee)
                        .ThenInclude(e => e.Schedule)
                        .ThenInclude(s => s.TimeSlots);
            }

            return await query.Where(s => ids.Contains(s.Id)).ToListAsync();
        }

        public async Task<List<Service>> GetAllByServiceGroupId(int serviceGroupId)
        {
            return await _context.Services.Where(s => s.ServiceGroupId == serviceGroupId).ToListAsync();
        }

        public async Task AddAsync(Service service)
        {
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Service service)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Service> services)
        {
            _context.Services.UpdateRange(services);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }
    }
}
