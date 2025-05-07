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
            CreateMap<CreateServiceDto, Service>();
            CreateMap<UpdateServiceDto, Service>();

            CreateMap<ServiceGroup, ServiceGroupDto>();
            CreateMap<CreateServiceGroupDto, ServiceGroup>();
            CreateMap<PatchServiceGroupDto, ServiceGroup>();
        }
    }
}
