using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public Roles Role { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
