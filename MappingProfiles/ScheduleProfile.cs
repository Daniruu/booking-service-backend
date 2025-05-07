using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<UpdateDayScheduleDto, DaySchedule>();
            CreateMap<DaySchedule, DayScheduleDto>();

            CreateMap<CreateTimeSlotDto, TimeSlot>();
            CreateMap<TimeSlot, TimeSlotDto>();
        }
    }
}
