namespace BookingService.DTOs
{
    public class EmployeePublicDto
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
