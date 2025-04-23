using BookingService.Models;

namespace BookingService.DTOs
{
    public class DayScheduleDto
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; }
    }
}
