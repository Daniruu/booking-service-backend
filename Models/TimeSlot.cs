namespace BookingService.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int DayScheduleId { get; set; }
        public DaySchedule DaySchedule { get; set; }
    }
}
