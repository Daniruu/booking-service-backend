namespace BookingService.Models
{
    public class BusinessSettings
    {
        public bool AutoConfirmBookings { get; set; } = false;
        public TimeSpan BookingBufferTime { get; set; } = TimeSpan.FromMinutes(15);
    }
}
