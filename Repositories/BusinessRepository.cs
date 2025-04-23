using BookingService.Data;
using BookingService.Models;
using BookingService.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly BookingServiceDbContext _context;

        public BusinessRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Business?> GetByIdAsync(int id, BusinessSpecifications? spec = null)
        {
            IQueryable<Business> query = _context.Businesses;

            if (spec?.IncludeAddress == true)
                query = query.Include(b => b.Address);

            if (spec?.IncludeRegistration == true)
                query = query.Include(b => b.RegistrationData);

            if (spec?.IncludeSettings == true)
                query = query.Include(b => b.Settings);

            if (spec?.IncludeSchedule  == true)
                query = query.Include(b => b.Schedule).ThenInclude(s => s.TimeSlots);

            if (spec?.IncludeImages == true)
                query = query.Include(b => b.Images);

            if (spec?.IncludeEmployees == true)
                query = query.Include(b => b.Employees).ThenInclude(e => e.Schedule).ThenInclude(s => s.TimeSlots);

            if (spec?.IncludeServices == true)
            {
                query = query.Include(b => b.ServiceGroups).ThenInclude(sg => sg.Services);
            }

            if (spec?.IncludeReviews == true)
                query = query.Include(b => b.Reviews)
                        .ThenInclude(r => r.User)
                        .Include(b => b.Reviews)
                        .ThenInclude(r => r.Images);

            if (spec?.IncludeBookings == true)
                query = query.Include(b => b.Bookings);

            return await query.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Business?> GetByEmailAsync(string email)
        {
            return await _context.Businesses.FirstOrDefaultAsync(b => b.Email == email);
        }

        public async Task<Business?> GetByNipAsync(string nip)
        {
            return await _context.Businesses.Include(b => b.RegistrationData).FirstOrDefaultAsync(b => b.RegistrationData.Nip == nip);
        }

        public async Task<Business?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Businesses.FirstOrDefaultAsync(b => b.Phone == phoneNumber);
        }

        public async Task UpdateAsync(Business business)
        {
            _context.Businesses.Update(business);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Business?>> GetPaginatedBusinessesAsync(int page, int limit, int? categoryId = null, string? location = null, string? searchTerms = null)
        {
            var query = _context.Businesses
                .Where(b => b.IsPublished)
                .Include(b => b.Address)
                .Include(b => b.Images.Where(i => i.IsPrimary))
                .Include(b => b.Reviews)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId);

            if (!string.IsNullOrEmpty(location))
                query = query.Where(b => b.Address.City.ToLower().Contains(location.ToLower()));

            if (!string.IsNullOrEmpty(searchTerms))
                query = query.Where(b => b.Name.ToLower().Contains(searchTerms.ToLower()));

            query = query.OrderBy(b => b.Id);

            return await query.Skip((page - 1) * limit).Take(limit).ToListAsync();
        }

        public async Task<int> CountBusinessesAsync(int? categoryId = null, string? location = null, string? searchTerms = null)
        {
            var query = _context.Businesses
                .Where(b => b.IsPublished)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId);

            if (!string.IsNullOrEmpty(location))
                query = query.Where(b => b.Address.City.ToLower().Contains(location.ToLower()));

            if (!string.IsNullOrEmpty(searchTerms))
                query = query.Where(b => b.Name.ToLower().Contains(searchTerms.ToLower()));

            return await query.CountAsync();
        }
    }
}
