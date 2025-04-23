using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ReviewCreateDto
    {

        [Required, Range(0, 5)]
        public int Rating { get; set; }

        [Required, MaxLength(500)]
        public string Comment { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
