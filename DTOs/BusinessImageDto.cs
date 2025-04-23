namespace BookingService.DTOs
{
    public class BusinessImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
    }
}
