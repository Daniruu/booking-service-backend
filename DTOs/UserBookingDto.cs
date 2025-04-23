using BookingService.Models;

namespace BookingService.DTOs
{
    public class UserBookingDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? Note { get; set; }
        public decimal FinalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public string ServiceName { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeAvatar { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessImage { get; set; }
    }
}
