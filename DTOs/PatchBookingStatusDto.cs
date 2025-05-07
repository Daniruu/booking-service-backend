using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class PatchBookingStatusDto
    {
        [Required]
        public BookingStatus Status { get; set; }
    }
}
