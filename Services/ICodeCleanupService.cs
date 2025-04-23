namespace BookingService.Services
{
    public interface ICodeCleanupService
    {
        Task CleanupExpiredCodesAsync();
    }
}
