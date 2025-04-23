using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, UserBookingDto>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
                .ForMember(dest => dest.EmployeeAvatar, opt => opt.MapFrom(src => src.Employee.AvatarUrl))
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.BusinessImage, opt => opt.MapFrom(src =>
                    src.Business.Images.Any(img => img.IsPrimary)
                    ? src.Business.Images.First(img => img.IsPrimary).Url
                    : src.Business.Images.FirstOrDefault().Url));

            CreateMap<Booking, BusinessBookingDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserPhone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.User.AvatarUrl))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));
        }
    }
}
