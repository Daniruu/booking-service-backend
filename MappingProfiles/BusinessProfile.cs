using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class BusinessProfile : Profile
    {
        public BusinessProfile()
        {
            CreateMap<Business, BusinessDto>();
            CreateMap<Business, BusinessListItemDto>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsPrimary)))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0));

            CreateMap<Business, BusinessPublicDetailsDto>()
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0));

            CreateMap<CreateBusinessDto, Business>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Roles.Business));

            CreateMap<PatchBusinessDto, Business>();

            CreateMap<UpdateAddressDto, Address>();

            CreateMap<PatchBusinessRegistrationDataDto, BusinessRegistrationData>();

            CreateMap<PatchBusinessSettingsDto, BusinessSettings>();

            CreateMap<BusinessImage, BusinessImageDto>();
        }
    }
}
