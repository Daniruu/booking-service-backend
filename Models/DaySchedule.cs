using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class DaySchedule
    {
        [Key]
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }

        public int? BusinessId { get; set; }
        public Business Business { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public List<TimeSlot> TimeSlots { get; set; } = new();
    }
}
