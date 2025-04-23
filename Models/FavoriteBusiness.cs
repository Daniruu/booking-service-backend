namespace BookingService.Models
{
    public class FavoriteBusiness
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }
    }
}
