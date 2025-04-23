using BookingService.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookingService.DTOs
{
    public class BusinessUpdateDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }


        public int? CategoryId { get; set; }
    }
}
