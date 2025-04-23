namespace BookingService.DTOs
{
    public class BookingCreateDto
    {
        public int ServiceId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public string? Note { get; set; }
    }
}
