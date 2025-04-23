using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class ReviewImage
    {
        public int Id { get; set; }

        public int ReviewId { get; set; }
        public Review Review { get; set; }

        [Required]
        public string Url { get; set; }

        public string? AltText { get; set; }
    }
}
