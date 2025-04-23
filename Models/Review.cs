using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        [Required, Range(0, 5)]
        public int Rating { get; set; }

        [Required, MaxLength(500)]
        public string Comment { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<ReviewImage> Images { get; set; } = new();
    }
}
