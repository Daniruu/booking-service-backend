namespace BookingService.DTOs
{
    public class CreateBookingDto
    {
        public int ServiceId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string? Note { get; set; }
    }
}
