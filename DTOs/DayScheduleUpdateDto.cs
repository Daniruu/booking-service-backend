using BookingService.Models;

namespace BookingService.DTOs
{
    public class DayScheduleUpdateDto
    {
        public DayOfWeek Day { get; set; }
        public List<AddTimeSlotDto> TimeSlots { get; set; }
    }
}
