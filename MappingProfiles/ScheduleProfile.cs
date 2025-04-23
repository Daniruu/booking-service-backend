using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<DayScheduleUpdateDto, DaySchedule>();
            CreateMap<DaySchedule, DayScheduleDto>();

            CreateMap<AddTimeSlotDto, TimeSlot>();
            CreateMap<TimeSlot, TimeSlotDto>();
        }
    }
}
