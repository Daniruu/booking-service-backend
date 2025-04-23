namespace BookingService.Specifications
{
    public class BookingSpecifications
    {
        public bool IncludeUser { get; set; } = false;
        public bool IncludeBusiness { get; set; } = false;
        public bool IncludeService { get; set; } = false;
        public bool IncludeEmployee { get; set; } = false;
    }
}
