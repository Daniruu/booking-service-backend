using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Service, ServiceDto>();
            CreateMap<ServiceCreateDto, Service>();
            CreateMap<ServiceUpdateDto, Service>();

            CreateMap<ServiceGroup, ServiceGroupDto>();
            CreateMap<ServiceGroupCreateDto, ServiceGroup>();
            CreateMap<ServiceGroupUpdateDto, ServiceGroup>();
        }
    }
}
