namespace BookingService.DTOs
{
    public class BusinessQueryParams
    {
        public int? CategoryId { get; set; }
        public string? City { get; set; }
        public string? SearchTerms { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
