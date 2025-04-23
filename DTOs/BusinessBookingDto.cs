using BookingService.Models;

namespace BookingService.DTOs
{
    public class BusinessBookingDto
    {
        public int Id { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? Note { get; set; }
        public decimal FinalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public int EmloyeeId { get; set; }
        public string EmployeeName { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
