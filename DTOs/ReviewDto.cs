using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? UserAvatarUrl { get; set; }

        public int BusinessId { get; set; }

        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }
}
