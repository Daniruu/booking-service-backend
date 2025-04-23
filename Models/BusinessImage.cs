using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class BusinessImage
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        [Required]
        public string Url { get; set; }

        public string? AltText { get; set; }
        public bool IsPrimary { get; set; } = false;
    }
}
