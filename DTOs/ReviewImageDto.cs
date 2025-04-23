using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ReviewImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? AltText { get; set; }
    }
}
