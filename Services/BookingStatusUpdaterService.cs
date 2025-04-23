using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BookingService.Services
{
    public class BookingStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingStatusUpdaterService(IServiceScopeFactory scopeFactor)
        {
            _scopeFactory = scopeFactor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<BookingServiceDbContext>();

                try
                {
                    var now = DateTimeOffset.UtcNow;

                    var expiredBookings = await context.Bookings
                        .Where(b => b.Status == BookingStatus.Active && b.EndTime < now)
                        .ToListAsync(stoppingToken);

                    foreach (var booking in expiredBookings)
                    {
                        booking.Status = BookingStatus.Complete;
                    }

                    if (expiredBookings.Any())
                    {
                        await context.SaveChangesAsync(stoppingToken);
                        Console.WriteLine($"[{now}] - Updated {expiredBookings.Count} bookings to 'Complete'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.UtcNow}] - Error updating bookings: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
