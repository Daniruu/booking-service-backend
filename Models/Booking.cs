using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public enum BookingStatus
    {
        Pending,
        Active,
        Canceled,
        Complete
    }
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ConfirmedAt { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Range(0, double.MaxValue)]
        public decimal FinalPrice { get; set; }

        public BookingStatus Status { get; set; }
    }
}
