using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BookingStatusUpdateDto
    {
        [Required]
        public BookingStatus Status { get; set; }
    }
}
