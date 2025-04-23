namespace BookingService.Specifications
{
    public class ServiceSpecifications
    {
        public bool IncludeBusiness { get; set; } = false;
        public bool IncludeEmployee { get; set; } = false;
        public bool IncludeServiceGroup { get; set; } = false;
    }
}
