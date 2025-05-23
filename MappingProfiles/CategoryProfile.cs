﻿using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateBusinessCategoryDto, BusinessCategory>()
                .ForMember(dest => dest.IconUrl, opt => opt.Ignore());

            CreateMap<BusinessCategory, BusinessCategoryDto>();
        }
    }
}
