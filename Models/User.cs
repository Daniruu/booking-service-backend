using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class User : Account
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Surname { get; set; }

        public string? AvatarUrl { get; set; }

        public List<Booking> Bookings { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<FavoriteBusiness> FavoriteBusinesses { get; set; } = new();
    }
}
