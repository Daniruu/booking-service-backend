using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.MappingProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile ()
        {
            CreateMap<Account, AccountDto>();
        }
    }
}
