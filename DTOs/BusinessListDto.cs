namespace BookingService.DTOs
{
    public class BusinessListDto
    {
        public List<BusinessListItemDto> BusinessList { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
