using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class BusinessFavoriteProfile : Profile
    {
        public BusinessFavoriteProfile()
        {
            CreateMap<FavoriteBusiness, BusinessFavoriteDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.BusinessId))
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
                .ForMember(dest => dest.BusinessPhone, opt => opt.MapFrom(src => src.Business.Phone))
                .ForMember(dest => dest.BusinessEmail, opt => opt.MapFrom(src => src.Business.Email))
                .ForMember(dest => dest.BusinessDescription, opt => opt.MapFrom(src => src.Business.Description))
                .ForMember(dest => dest.BusinessCategoryId, opt => opt.MapFrom(src => src.Business.CategoryId))
                .ForMember(dest => dest.BusinessAddress, opt => opt.MapFrom(src => src.Business.Address))
                .ForMember(dest => dest.BusinessImage, opt => opt.MapFrom(src => 
                    src.Business.Images.Any(img => img.IsPrimary)
                    ? src.Business.Images.First(img => img.IsPrimary).Url
                    : src.Business.Images.FirstOrDefault().Url));
                    
        }
    }
}
