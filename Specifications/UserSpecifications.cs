namespace BookingService.Specifications
{
    public class UserSpecifications
    {
        public bool IncludeBookings { get; set; } = false;
        public bool IncludeReviews { get; set; } = false;
        public bool IncludeFavorites { get; set; } = false;
    }
}
