using BookingService.Models;

namespace BookingService.DTOs
{
    public class UpdateDayScheduleDto
    {
        public DayOfWeek Day { get; set; }
        public List<CreateTimeSlotDto> TimeSlots { get; set; }
    }
}
